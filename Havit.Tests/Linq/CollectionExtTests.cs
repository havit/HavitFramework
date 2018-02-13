using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Havit.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Tests.Linq
{
	[TestClass]
	public class CollectionExtTests
	{
		[TestMethod]
		public void CollectionExt_UpdateFrom_Demo()
		{
			// arrange
			TargetClass targetItem1 = new TargetClass() { Id = 1, StringProperty = "FAKE_TARGET_1" };
			TargetClass targetItem3 = new TargetClass() { Id = 3, StringProperty = "FAKE_TARGET_3" };
			var targetList = new List<TargetClass>()
			{
				targetItem1,
				targetItem3
			};
			var sourceList = new List<SourceClass>()
			{
				new SourceClass() { Id = 1, StringProperty = "FAKE_SOURCE_1" },
				new SourceClass() { Id = 2, StringProperty = "FAKE_SOURCE_2" }
			};

			// act
			var result = targetList.UpdateFrom(sourceList,
				targetKeySelector: target => target.Id,
				sourceKeySelector: source => source.Id,
				newItemCreateFunc:
					source =>
					{
						var newItem = new TargetClass()
						{
							Id = source.Id,
							StringProperty = source.StringProperty
						};
						return newItem;
					},
				updateItemAction:
					(source, target) =>
					{
						target.StringProperty = source.StringProperty;
					},
				removeItemAction:
					(target) =>
					{
					}
			);
			// unitOfWork.AddForInsert(result.ItemsAdding);
			// unitOfWork.AddForUpdate(result.ItemsUpdating);
			// unitOfWork.AddForDelete(result.ItemsRemoving);

			// assert
			// Item 1 updated
			CollectionAssert.Contains(targetList, targetItem1);
			var item1 = targetList.Single(i => i.Id == 1);
			Assert.AreEqual("FAKE_SOURCE_1", item1.StringProperty);
			Assert.AreSame(targetItem1, result.ItemsUpdating.Single());

			// Item 3 removed
			CollectionAssert.DoesNotContain(targetList, targetItem3);
			Assert.AreSame(targetItem3, result.ItemsRemoving.Single());

			// Item 2 added
			Assert.AreEqual(2, targetList.Count);
			var item2 = targetList.Single(i => i.Id == 2);
			Assert.AreEqual("FAKE_SOURCE_2", item2.StringProperty);
			Assert.AreSame(item2, result.ItemsAdding.Single());
		}

		[TestMethod]
		public void CollectionExt_UpdateFrom_NullKey_ShouldMatchNullToNull()
		{
			// arrange
			TargetClass targetItem1 = new TargetClass() { StringProperty = null, OtherProperty = "TARGET_OTHER_1" };
			TargetClass targetItem3 = new TargetClass() { StringProperty = "TARGET_KEY_3", OtherProperty = "TARGET_OTHER_3" };
			var targetList = new List<TargetClass>()
			{
				targetItem1,
				targetItem3
			};
			var sourceList = new List<SourceClass>()
			{
				new SourceClass() { StringProperty = null, OtherProperty = 0 },
				new SourceClass() { StringProperty = "SOURCE_KEY_2", OtherProperty = 2 }
			};

			// act
			var result = targetList.UpdateFrom(sourceList,
				targetKeySelector: target => target.StringProperty,
				sourceKeySelector: source => source.StringProperty,
				newItemCreateFunc:
					source =>
					{
						var newItem = new TargetClass()
						{
							StringProperty = source.StringProperty,
							OtherProperty = source.OtherProperty.ToString()
						};
						return newItem;
					},
				updateItemAction:
					(source, target) =>
					{
						target.StringProperty = source.StringProperty;
						target.OtherProperty = source.OtherProperty.ToString();
					},
				removeItemAction:
					(target) => { }
			);

			// assert
			// Item 1 updated
			CollectionAssert.Contains(targetList, targetItem1);
			var item1 = targetList.Single(i => i.StringProperty == null);
			Assert.AreEqual("0", item1.OtherProperty);
			Assert.AreSame(item1, result.ItemsUpdating.Single());

			// Item 3 removed
			CollectionAssert.DoesNotContain(targetList, targetItem3);
			Assert.AreSame(targetItem3, result.ItemsRemoving.Single());

			// Item 2 added
			Assert.AreEqual(2, targetList.Count);
			var item2 = targetList.Single(i => i.StringProperty == "SOURCE_KEY_2");
			Assert.AreEqual("2", item2.OtherProperty);
			Assert.AreSame(item2, result.ItemsAdding.Single());
		}

		[TestMethod]
		public void CollectionExt_UpdateFrom_DuplicateSourceKey_DoubleUpdate()
		{
			// arrange
			TargetClass targetItem1 = new TargetClass() { Id = 1, StringProperty = "FAKE_TARGET_1" };
			var targetList = new List<TargetClass>()
			{
				targetItem1,
			};
			var sourceList = new List<SourceClass>()
			{
				new SourceClass() { Id = 1, StringProperty = "FAKE_SOURCE_11" },
				new SourceClass() { Id = 1, StringProperty = "FAKE_SOURCE_12" }
			};

			var itemsAdded = new List<TargetClass>();
			var itemsUpdated = new List<TargetClass>();
			var itemsRemoved = new List<TargetClass>();

			// act
			var result = targetList.UpdateFrom(sourceList,
				targetKeySelector: target => target.Id,
				sourceKeySelector: source => source.Id,
				newItemCreateFunc:
					source =>
					{
						var newItem = new TargetClass()
						{
							Id = source.Id,
							StringProperty = source.StringProperty
						};
						itemsAdded.Add(newItem);
						return newItem;
					},
				updateItemAction:
					(source, target) =>
					{
						target.StringProperty = source.StringProperty;
						itemsUpdated.Add(target);
					},
				removeItemAction:
					(target) =>
					{
						itemsRemoved.Add(target);
					}
			);

			// assert
			// Item 1 updated
			CollectionAssert.Contains(targetList, targetItem1);
			var item1 = targetList.Single(i => i.Id == 1);
			Assert.AreEqual("FAKE_SOURCE_12", item1.StringProperty);
			Assert.AreEqual(0, itemsAdded.Count);
			Assert.AreEqual(0, result.ItemsAdding.Count);
			Assert.AreEqual(2, itemsUpdated.Count);
			Assert.AreEqual(2, result.ItemsUpdating.Count);
			Assert.AreSame(result.ItemsUpdating[0], result.ItemsUpdating[1]);
			Assert.AreEqual(0, itemsRemoved.Count);
			Assert.AreEqual(0, result.ItemsRemoving.Count);
		}

		[TestMethod]
		public void CollectionExt_UpdateFrom_DuplicateTargetKey_ShouldUpdateBoth()
		{
			// arrange
			TargetClass targetItem1 = new TargetClass() { Id = 1, StringProperty = "FAKE_TARGET_1" };
			TargetClass targetItem2 = new TargetClass() { Id = 1, StringProperty = "FAKE_TARGET_2" };
			var targetList = new List<TargetClass>()
			{
				targetItem1,
				targetItem2,
			};
			var sourceList = new List<SourceClass>()
			{
				new SourceClass() { Id = 1, StringProperty = "FAKE_SOURCE_1" },
			};

			var itemsAdded = new List<TargetClass>();
			var itemsUpdated = new List<TargetClass>();
			var itemsRemoved = new List<TargetClass>();

			// act
			var result = targetList.UpdateFrom(sourceList,
				targetKeySelector: target => target.Id,
				sourceKeySelector: source => source.Id,
				newItemCreateFunc:
					source =>
					{
						var newItem = new TargetClass()
						{
							Id = source.Id,
							StringProperty = source.StringProperty
						};
						itemsAdded.Add(newItem);
						return newItem;
					},
				updateItemAction:
					(source, target) =>
					{
						target.StringProperty = source.StringProperty;
						itemsUpdated.Add(target);
					},
				removeItemAction:
					(target) =>
					{
						itemsRemoved.Add(target);
					}
			);

			// assert
			// Item 1 updated
			CollectionAssert.Contains(targetList, targetItem1);
			CollectionAssert.Contains(targetList, targetItem2);
			Assert.AreEqual(2, targetList.Count);
			Assert.AreEqual("FAKE_SOURCE_1", targetList[0].StringProperty);
			Assert.AreEqual("FAKE_SOURCE_1", targetList[1].StringProperty);
			Assert.AreEqual(0, itemsAdded.Count);
			Assert.AreEqual(0, result.ItemsAdding.Count);
			Assert.AreEqual(2, itemsUpdated.Count);
			Assert.AreEqual(2, result.ItemsUpdating.Count);
			CollectionAssert.Contains(result.ItemsUpdating, targetItem1);
			CollectionAssert.Contains(result.ItemsUpdating, targetItem2);
			Assert.AreEqual(0, itemsRemoved.Count);
			Assert.AreEqual(0, result.ItemsRemoving.Count);
		}

		[TestMethod]
		public void CollectionExt_UpdateFrom_NullNewItemFunc_ShouldNotAddNewItems()
		{
			// arrange
			var targetList = new List<TargetClass>()
			{
				new TargetClass() { Id = 1, StringProperty = "ITEM_TO_UPDATE" },
				new TargetClass() { Id = 3, StringProperty = "ITEM_TO_REMOVE" },
			};
			var sourceList = new List<SourceClass>()
			{
				new SourceClass() { Id = 1, StringProperty = "FAKE_SOURCE_1" },
				new SourceClass() { Id = 2, StringProperty = "FAKE_SOURCE_2" }
			};

			// act
			var result = targetList.UpdateFrom(sourceList,
				targetKeySelector: target => target.Id,
				sourceKeySelector: source => source.Id,
				newItemCreateFunc: null,
				updateItemAction:
					(source, target) =>
					{
						target.StringProperty = source.StringProperty;
					},
				removeItemAction:
					(target) =>
					{
					}
			);

			// assert
			Assert.AreEqual(1, targetList.Count);
			Assert.AreEqual("FAKE_SOURCE_1", targetList.Single().StringProperty);
			Assert.AreEqual(0, result.ItemsAdding.Count);
		}

		[TestMethod]
		public void CollectionExt_UpdateFrom_NullUpdateAction_ShouldNotUpdateItems()
		{
			// arrange
			var targetList = new List<TargetClass>()
			{
				new TargetClass() { Id = 1, StringProperty = "ITEM_TO_UPDATE" },
				new TargetClass() { Id = 3, StringProperty = "ITEM_TO_REMOVE" },
			};
			var sourceList = new List<SourceClass>()
			{
				new SourceClass() { Id = 1, StringProperty = "FAKE_SOURCE_1" },
				new SourceClass() { Id = 2, StringProperty = "FAKE_SOURCE_2" }
			};

			// act
			var result = targetList.UpdateFrom(sourceList,
				targetKeySelector: target => target.Id,
				sourceKeySelector: source => source.Id,
				newItemCreateFunc:
					source =>
					{
						var newItem = new TargetClass()
						{
							Id = source.Id,
							StringProperty = source.StringProperty
						};
						return newItem;
					},
				updateItemAction: null,
				removeItemAction:
					(target) =>
					{
					}
			);

			// assert
			Assert.AreEqual(2, targetList.Count);
			Assert.AreEqual("ITEM_TO_UPDATE", targetList.Single(i => i.Id == 1).StringProperty);
			Assert.AreEqual(1, targetList.Count(i => i.Id == 2));
			Assert.AreEqual(0, result.ItemsUpdating.Count);
		}

		[TestMethod]
		public void CollectionExt_UpdateFrom_NullRemoveAction_ShouldNotRemoveItems()
		{
			// arrange
			var targetList = new List<TargetClass>()
			{
				new TargetClass() { Id = 1, StringProperty = "ITEM_TO_UPDATE" },
				new TargetClass() { Id = 3, StringProperty = "ITEM_TO_REMOVE" },
			};
			var sourceList = new List<SourceClass>()
			{
				new SourceClass() { Id = 1, StringProperty = "FAKE_SOURCE_1" },
				new SourceClass() { Id = 2, StringProperty = "FAKE_SOURCE_2" }
			};

			// act
			var result = targetList.UpdateFrom(sourceList,
				targetKeySelector: target => target.Id,
				sourceKeySelector: source => source.Id,
				newItemCreateFunc:
					source =>
					{
						var newItem = new TargetClass()
						{
							Id = source.Id,
							StringProperty = source.StringProperty
						};
						return newItem;
					},
				updateItemAction:
					(source, target) =>
					{
						target.StringProperty = source.StringProperty;
					},
				removeItemAction: null
			);

			// assert
			Assert.AreEqual(3, targetList.Count);
			Assert.AreEqual(1, targetList.Count(i => i.Id == 1));
			Assert.AreEqual(1, targetList.Count(i => i.Id == 2));
			Assert.AreEqual(1, targetList.Count(i => i.Id == 3));
			Assert.AreEqual(0, result.ItemsRemoving.Count);
		}

		[TestMethod]
		public void CollectionExt_UpdateFrom_StructSource()
		{
			// arrange
			TargetClass targetItem1 = new TargetClass() { Id = 1, StringProperty = "FAKE_TARGET_1" };
			TargetClass targetItem3 = new TargetClass() { Id = 3, StringProperty = "FAKE_TARGET_3" };
			var targetList = new List<TargetClass>()
			{
				targetItem1,
				targetItem3
			};
			var sourceList = new List<SourceStruct>()
			{
				new SourceStruct() { Id = 1, StringProperty = "FAKE_SOURCE_1" },
				new SourceStruct() { Id = 2, StringProperty = "FAKE_SOURCE_2" }
			};

			// act
			var result = targetList.UpdateFrom(sourceList,
				targetKeySelector: target => target.Id,
				sourceKeySelector: source => source.Id,
				newItemCreateFunc:
					source =>
					{
						var newItem = new TargetClass()
						{
							Id = source.Id,
							StringProperty = source.StringProperty
						};
						return newItem;
					},
				updateItemAction:
					(source, target) =>
					{
						target.StringProperty = source.StringProperty;
					},
				removeItemAction:
					(target) => { }
			);

			// assert
			// Item 1 updated
			CollectionAssert.Contains(targetList, targetItem1);
			var item1 = targetList.Single(i => i.Id == 1);
			Assert.AreEqual("FAKE_SOURCE_1", item1.StringProperty);
			Assert.AreSame(targetItem1, result.ItemsUpdating.Single());

			// Item 3 removed
			CollectionAssert.DoesNotContain(targetList, targetItem3);
			Assert.AreSame(targetItem3, result.ItemsRemoving.Single());

			// Item 2 added
			Assert.AreEqual(2, targetList.Count);
			var item2 = targetList.Single(i => i.Id == 2);
			Assert.AreEqual("FAKE_SOURCE_2", item2.StringProperty);
			Assert.AreSame(item2, result.ItemsAdding.Single());
		}

		[TestMethod]
		public void CollectionExt_UpdateFrom_CompositeKey()
		{
			// arrange
			TargetClass targetItem1 = new TargetClass() { Id = 1, StringProperty = "KEY_1", OtherProperty = "TARGET_1" };
			TargetClass targetItem3 = new TargetClass() { Id = 3, StringProperty = "KEY_3", OtherProperty = "TARGET_3" };
			var targetList = new List<TargetClass>()
			{
				targetItem1,
				targetItem3
			};
			var sourceList = new List<SourceClass>()
			{
				new SourceClass() { Id = 1, StringProperty = "KEY_1", OtherProperty = 1 },
				new SourceClass() { Id = 2, StringProperty = "KEY_2", OtherProperty = 2 }
			};

			// act
			var result = targetList.UpdateFrom(sourceList,
				targetKeySelector: target => new { Id1 = target.Id, Id2 = target.StringProperty },
				sourceKeySelector: source => new { Id1 = source.Id, Id2 = source.StringProperty },
				newItemCreateFunc:
					source =>
					{
						var newItem = new TargetClass()
						{
							Id = source.Id,
							StringProperty = source.StringProperty,
							OtherProperty = source.OtherProperty.ToString()
						};
						return newItem;
					},
				updateItemAction:
					(source, target) =>
					{
						target.OtherProperty = source.OtherProperty.ToString();
					},
				removeItemAction:
					(target) =>
					{
					}
			);

			// assert
			// Item 1 updated
			CollectionAssert.Contains(targetList, targetItem1);
			var item1 = targetList.Single(i => i.Id == 1);
			Assert.AreEqual("1", item1.OtherProperty);
			Assert.AreSame(targetItem1, result.ItemsUpdating.Single());

			// Item 3 removed
			CollectionAssert.DoesNotContain(targetList, targetItem3);
			Assert.AreSame(targetItem3, result.ItemsRemoving.Single());

			// Item 2 added
			Assert.AreEqual(2, targetList.Count);
			var item2 = targetList.Single(i => i.Id == 2);
			Assert.AreEqual("2", item2.OtherProperty);
			Assert.AreSame(item2, result.ItemsAdding.Single());
		}

		[TestMethod]
		public void CollectionExt_UpdateFrom_RemoveItemFromCollectionFalse()
		{
			// arrange
			var targetList = new List<TargetClass>()
			{
				new TargetClass() { Id = 1, StringProperty = "FAKE_TARGET_1" },
				new TargetClass() { Id = 3, StringProperty = "FAKE_TARGET_3" }
			};
			var sourceList = new List<SourceClass>();

			// act
			var result = targetList.UpdateFrom(sourceList,
				targetKeySelector: target => target.Id,
				sourceKeySelector: source => source.Id,
				newItemCreateFunc: source => null,
				updateItemAction: (source, target) => {	},
				removeItemAction: (target) => { },
				removeItemFromCollection: false
			);

			// assert
			Assert.AreEqual(2, targetList.Count);
			Assert.AreEqual(2, result.ItemsRemoving.Count);
		}

		private class SourceClass
		{
			public int Id { get; set; }
			public string StringProperty { get; set; }
			public int OtherProperty { get; set; }
		}

		private class TargetClass
		{
			public int Id { get; set; }
			public string StringProperty { get; set; }
			public string OtherProperty { get; set; }
		}

		private struct SourceStruct
		{
			public int Id { get; set; }
			public string StringProperty { get; set; }
		}
	}
}
