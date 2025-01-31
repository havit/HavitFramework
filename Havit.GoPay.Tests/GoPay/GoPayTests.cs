using System.Net;
using System.Net.Http;
using Havit.GoPay;
using Havit.GoPay.Codebooks;
using Havit.GoPay.DataObjects;
using Havit.GoPay.DataObjects.Errors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Havit.Tests.GoPay;

[TestClass]
public class GoPayTests
{
	[Ignore]
	//[TestMethod]   TestsForLocalDebugging?
	public void GoPayClient_GetTestToken()
	{
		HttpClient httpClient = new HttpClient
		{
			BaseAddress = new Uri("https://gw.sandbox.gopay.com/api")
		};

		using (IGoPayClient client = new GoPayClient(httpClient))
		{
			// ! reálný testovací účet bookport.cz !
			GoPayResponse response = client.GetToken("1078082522", "dztf89xH", GoPayPaymentScope.PaymentAll);
		}
	}

	[TestMethod]
	public void GoPayClient_GetToken_ReturnsTokenWithCorrectCredentials()
	{
		// arrange
		object apiResponse = new
		{
			access_token = Guid.NewGuid(),
			token_type = "Bearer",
			expires_in = 1800
		};

		MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
		mockHttp
			.Expect(HttpMethod.Post, "https://fake.com/api/oauth2/token")
			.WithFormData("grant_type", "client_credentials")
			.WithFormData("scope", "payment-create")
			.WithHeaders("Authorization", "Basic RkFLRTpGQUtF")
			.Respond(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(JsonConvert.SerializeObject(apiResponse))
			});

		HttpClient httpClient = new HttpClient(mockHttp) { BaseAddress = new Uri("https://fake.com/api") };

		GoPayResponse response;
		using (IGoPayClient client = new GoPayClient(httpClient))
		{
			// act
			response = client.GetToken("FAKE", "FAKE", GoPayPaymentScope.PaymentCreate);
		}

		// assert
		mockHttp.VerifyNoOutstandingRequest();
		mockHttp.VerifyNoOutstandingExpectation();

		Assert.IsFalse(response.HasErrors);
		Assert.IsTrue(!String.IsNullOrEmpty(response.AccessToken));
		Assert.IsTrue(response.TokenExpiresInSeconds > 0);
		Assert.IsTrue(!String.IsNullOrEmpty(response.TokenType));
	}

	[TestMethod]
	public void GoPayClient_GetToken_ReturnsErrorWithBadCredentials()
	{
		// arrange
		object apiResponse = new
		{
			date = DateTime.Now.Ticks,
			errors = new[]
			{
				new
				{
					scope = "G",
					error_code = 202,
					description = "Wrong credentials."
				}
			}
		};

		MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
		mockHttp
			.Expect(HttpMethod.Post, "https://fake.com/api/oauth2/token")
			.WithFormData("grant_type", "client_credentials")
			.WithFormData("scope", "payment-create")
			.WithHeaders("Authorization", "Basic QkFEOkNSRURFTlRJQUxT")
			.Respond(new HttpResponseMessage
			{
				StatusCode = (HttpStatusCode)403,
				Content = new StringContent(JsonConvert.SerializeObject(apiResponse))
			});

		HttpClient httpClient = new HttpClient(mockHttp)
		{
			BaseAddress = new Uri("https://fake.com/api")
		};

		GoPayResponse response;
		using (IGoPayClient client = new GoPayClient(httpClient))
		{
			// act
			response = client.GetToken("BAD", "CREDENTIALS", GoPayPaymentScope.PaymentCreate);
		}

		// assert
		mockHttp.VerifyNoOutstandingRequest();
		mockHttp.VerifyNoOutstandingExpectation();

		Assert.IsTrue(response.HasErrors);
		Assert.AreEqual(GoPayResponseErrorType.BadCredentials, response.Errors[0].ErrorType);
	}

	[TestMethod]
	public void GoPayClient_CreatePayment_ReturnsResponseWithCreatedPayment()
	{
		// arrange
		object payment = CreateFakePaymentResponse();

		string apiResponse = JsonConvert.SerializeObject(payment);

		MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
		mockHttp
			.Expect(HttpMethod.Post, "https://fake.com/api/payments/payment")
			.WithHeaders("Authorization", "Bearer QkFEOkNSRURFTlRJQUxT")
			.Respond(new HttpResponseMessage
			{
				StatusCode = (HttpStatusCode)200,
				Content = new StringContent(apiResponse)
			});

		HttpClient httpClient = new HttpClient(mockHttp)
		{
			BaseAddress = new Uri("https://fake.com/api")
		};

		GoPayRequest request = CreatePaymentRequest("QkFEOkNSRURFTlRJQUxT");

		GoPayResponse response;
		using (IGoPayClient client = new GoPayClient(httpClient))
		{
			// act
			response = client.CreatePayment(request);
		}

		// assert
		mockHttp.VerifyNoOutstandingRequest();
		mockHttp.VerifyNoOutstandingExpectation();

		Assert.IsFalse(response.HasErrors);
		Assert.AreEqual(GoPayPaymentState.Created, response.State);
	}

	[TestMethod]
	public void GoPayClient_CreatePayment_ReturnsResponseWithRequestedPreauthorization()
	{
		// arrange
		object payment = CreateFakePaymentPreauthorizedResponse();

		MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
		mockHttp
			.Expect(HttpMethod.Post, "https://fake.com/api/payments/payment")
			.WithHeaders("Authorization", "Bearer QkFEOkNSRURFTlRJQUxT")
			.Respond(new HttpResponseMessage
			{
				StatusCode = (HttpStatusCode)200,
				Content = new StringContent(JsonConvert.SerializeObject(payment))
			});

		HttpClient httpClient = new HttpClient(mockHttp)
		{
			BaseAddress = new Uri("https://fake.com/api")
		};

		GoPayRequest request = CreatePreauthorizedPaymentRequest("QkFEOkNSRURFTlRJQUxT");
		GoPayResponse response;
		using (IGoPayClient client = new GoPayClient(httpClient))
		{
			// act
			response = client.CreatePayment(request);
		}

		// assert
		mockHttp.VerifyNoOutstandingRequest();
		mockHttp.VerifyNoOutstandingExpectation();

		Assert.IsFalse(response.HasErrors);
		Assert.IsTrue(response.Preauthorization.Requested);
		Assert.AreEqual(GoPayPaymentState.Requested, response.Preauthorization.State);
	}

	[TestMethod]
	public void GoPayClient_CreatePayment_ReturnsResponseWithRecurrentPayment()
	{
		// arrange
		object payment = CreateFakeRecurrentPaymentResponse();

		MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
		mockHttp
			.Expect(HttpMethod.Post, "https://fake.com/api/payments/payment")
			.WithHeaders("Authorization", "Bearer QkFEOkNSRURFTlRJQUxT")
			.Respond(new HttpResponseMessage
			{
				StatusCode = (HttpStatusCode)200,
				Content = new StringContent(JsonConvert.SerializeObject(payment))
			});

		HttpClient httpClient = new HttpClient(mockHttp)
		{
			BaseAddress = new Uri("https://fake.com/api")
		};

		GoPayRequest request = CreateRecurrentPaymentRequest("QkFEOkNSRURFTlRJQUxT");

		GoPayResponse response;
		using (IGoPayClient client = new GoPayClient(httpClient))
		{
			// act
			response = client.CreatePayment(request);
		}

		// assert
		mockHttp.VerifyNoOutstandingRequest();
		mockHttp.VerifyNoOutstandingExpectation();

		Assert.IsFalse(response.HasErrors);
		Assert.IsTrue(response.Recurrence != null);
		Assert.AreEqual(GoPayRecurrenceState.Requested, response.Recurrence.State);
	}

	[TestMethod]
	public void GoPayClient_GetPayment_ReturnsResponseExistingPayment()
	{
		// arrange
		object payment = CreateFakePaymentResponse();

		MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
		mockHttp
			.Expect(HttpMethod.Get, "https://fake.com/api/payments/payment/3000006529")
			.WithHeaders("Authorization", "Bearer QkFEOkNSRURFTlRJQUxT")
			.Respond(new HttpResponseMessage
			{
				StatusCode = (HttpStatusCode)200,
				Content = new StringContent(JsonConvert.SerializeObject(payment))
			});

		HttpClient httpClient = new HttpClient(mockHttp)
		{
			BaseAddress = new Uri("https://fake.com/api")
		};

		GoPayResponse response;
		using (IGoPayClient client = new GoPayClient(httpClient))
		{
			// act
			response = client.GetPayment(3000006529, "QkFEOkNSRURFTlRJQUxT");
		}

		// assert
		mockHttp.VerifyNoOutstandingRequest();
		mockHttp.VerifyNoOutstandingExpectation();

		Assert.IsFalse(response.HasErrors);
		Assert.AreEqual(3000006529, response.Id);
	}

	[TestMethod]
	public void GoPayClient_RefundPayment_RefundsPaymentPartially()
	{
		// arrange
		object payment = new
		{
			id = 3041023648,
			result = "FINISHED"
		};

		MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
		mockHttp
			.Expect(HttpMethod.Post, "https://fake.com/api/payments/payment/3041023648/refund")
			.WithHeaders("Authorization", "Bearer QkFEOkNSRURFTlRJQUxT")
			.Respond(new HttpResponseMessage
			{
				StatusCode = (HttpStatusCode)200,
				Content = new StringContent(JsonConvert.SerializeObject(payment))
			});

		HttpClient httpClient = new HttpClient(mockHttp)
		{
			BaseAddress = new Uri("https://fake.com/api")
		};

		GoPayResponse response;
		using (IGoPayClient client = new GoPayClient(httpClient))
		{
			// act
			response = client.RefundPayment(3041023648, 100, "QkFEOkNSRURFTlRJQUxT");
		}

		// assert
		mockHttp.VerifyNoOutstandingRequest();
		mockHttp.VerifyNoOutstandingExpectation();

		Assert.IsFalse(response.HasErrors);
		Assert.AreEqual(GoPayOperationResult.Finished, response.Result);
	}

	[TestMethod]
	public void GoPayClient_RefundPayment_RefundsPaymentFully()
	{
		// arrange
		object payment = new
		{
			id = 3041023648,
			result = "FINISHED"
		};

		MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
		mockHttp
			.Expect(HttpMethod.Post, "https://fake.com/api/payments/payment/3041023648/refund")
			.WithHeaders("Authorization", "Bearer QkFEOkNSRURFTlRJQUxT")
			.Respond(new HttpResponseMessage
			{
				StatusCode = (HttpStatusCode)200,
				Content = new StringContent(JsonConvert.SerializeObject(payment))
			});

		HttpClient httpClient = new HttpClient(mockHttp)
		{
			BaseAddress = new Uri("https://fake.com/api")
		};

		GoPayResponse response;
		using (IGoPayClient client = new GoPayClient(httpClient))
		{
			// act
			response = client.RefundPayment(3041023648, 1000, "QkFEOkNSRURFTlRJQUxT");
		}

		// assert
		mockHttp.VerifyNoOutstandingRequest();
		mockHttp.VerifyNoOutstandingExpectation();

		Assert.IsFalse(response.HasErrors);
		Assert.AreEqual(GoPayOperationResult.Finished, response.Result);
	}

	[TestMethod]
	public void GoPayClient_CreateRecurrentPaymentOnDemand_ReturnRecurrenctPaymentOnDemand_BasedOnParentPayment()
	{
		// arrange
		object payment = CreateFakeRecurrentOnDemandPaymentResponse();

		MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
		mockHttp
			.Expect(HttpMethod.Post, "https://fake.com/api/payments/payment/3000006542/create-recurrence")
			.WithHeaders("Authorization", "Bearer QkFEOkNSRURFTlRJQUxT")
			.Respond(new HttpResponseMessage
			{
				StatusCode = (HttpStatusCode)200,
				Content = new StringContent(JsonConvert.SerializeObject(payment))
			});

		HttpClient httpClient = new HttpClient(mockHttp)
		{
			BaseAddress = new Uri("https://fake.com/api")
		};

		GoPayRequest request = CreateRecurentPaymentOnDemandRequest("QkFEOkNSRURFTlRJQUxT");

		GoPayResponse response;
		using (IGoPayClient client = new GoPayClient(httpClient))
		{
			// act
			response = client.CreateRecurrentPaymentOnDemand(3000006542, request);
		}

		// assert
		mockHttp.VerifyNoOutstandingRequest();
		mockHttp.VerifyNoOutstandingExpectation();

		Assert.IsFalse(response.HasErrors);
		Assert.AreEqual(3000006542, response.ParentId);
	}

	[TestMethod]
	public void GoPayClient_CancelRecurrentPayment_ReturnsResponseWithResult()
	{
		// arrange
		object payment = new
		{
			id = 3041023648,
			result = "FINISHED"
		};

		MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
		mockHttp
			.Expect(HttpMethod.Post, "https://fake.com/api/payments/payment/3041023648/void-recurrence")
			.WithHeaders("Authorization", "Bearer QkFEOkNSRURFTlRJQUxT")
			.Respond(new HttpResponseMessage
			{
				StatusCode = (HttpStatusCode)200,
				Content = new StringContent(JsonConvert.SerializeObject(payment))
			});

		HttpClient httpClient = new HttpClient(mockHttp)
		{
			BaseAddress = new Uri("https://fake.com/api")
		};

		GoPayResponse response;
		using (IGoPayClient client = new GoPayClient(httpClient))
		{
			// act
			response = client.CancelRecurrentPayment(3041023648, "QkFEOkNSRURFTlRJQUxT");
		}

		// assert
		mockHttp.VerifyNoOutstandingRequest();
		mockHttp.VerifyNoOutstandingExpectation();

		Assert.IsFalse(response.HasErrors);
		Assert.AreEqual(GoPayOperationResult.Finished, response.Result);
	}

	[TestMethod]
	public void GoPayClient_CancelPreauthorizedPayment_ReturnsResponseWithResult()
	{
		// arrange
		object payment = new
		{
			id = 3041023648,
			result = "FINISHED"
		};

		MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
		mockHttp
			.Expect(HttpMethod.Post, "https://fake.com/api/payments/payment/3041023648/void-authorization")
			.WithHeaders("Authorization", "Bearer QkFEOkNSRURFTlRJQUxT")
			.Respond(new HttpResponseMessage
			{
				StatusCode = (HttpStatusCode)200,
				Content = new StringContent(JsonConvert.SerializeObject(payment))
			});

		HttpClient httpClient = new HttpClient(mockHttp)
		{
			BaseAddress = new Uri("https://fake.com/api")
		};

		GoPayResponse response;
		using (IGoPayClient client = new GoPayClient(httpClient))
		{
			// act
			response = client.CancelPreauthorizedPayment(3041023648, "QkFEOkNSRURFTlRJQUxT");
		}

		// assert
		mockHttp.VerifyNoOutstandingRequest();
		mockHttp.VerifyNoOutstandingExpectation();

		Assert.IsFalse(response.HasErrors);
		Assert.AreEqual(GoPayOperationResult.Finished, response.Result);
	}

	[TestMethod]
	public void GoPayClient_CapturePayment_ReturnsResponseWithResult()
	{
		// arrange
		object payment = new
		{
			id = 3041023648,
			result = "FINISHED"
		};

		MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
		mockHttp
			.Expect(HttpMethod.Post, "https://fake.com/api/payments/payment/3041023648/capture")
			.WithHeaders("Authorization", "Bearer QkFEOkNSRURFTlRJQUxT")
			.Respond(new HttpResponseMessage
			{
				StatusCode = (HttpStatusCode)200,
				Content = new StringContent(JsonConvert.SerializeObject(payment))
			});

		HttpClient httpClient = new HttpClient(mockHttp)
		{
			BaseAddress = new Uri("https://fake.com/api")
		};

		GoPayResponse response;
		using (IGoPayClient client = new GoPayClient(httpClient))
		{
			// act
			response = client.CapturePayment(3041023648, "QkFEOkNSRURFTlRJQUxT");
		}

		// assert
		mockHttp.VerifyNoOutstandingRequest();
		mockHttp.VerifyNoOutstandingExpectation();

		Assert.IsFalse(response.HasErrors);
		Assert.AreEqual(GoPayOperationResult.Finished, response.Result);
	}

	[TestMethod]
	public void GoPayClient_GetAllowedPaymentMethods_ReturnsResponseWithAllowedPaymentMethods()
	{
		// arrange
		object payment = CreateFakeAllowedPaymentMethodsResponse();

		MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
		mockHttp
			.Expect(HttpMethod.Get, "https://fake.com/api/eshops/eshop/8657786874/payment-instruments")
			.WithHeaders("Authorization", "Bearer QkFEOkNSRURFTlRJQUxT")
			.Respond(new HttpResponseMessage
			{
				StatusCode = (HttpStatusCode)200,
				Content = new StringContent(JsonConvert.SerializeObject(payment))
			});

		HttpClient httpClient = new HttpClient(mockHttp)
		{
			BaseAddress = new Uri("https://fake.com/api")
		};

		GoPayResponse response;
		using (IGoPayClient client = new GoPayClient(httpClient))
		{
			// act
			response = client.GetAllowedPaymentMethods(8657786874, "QkFEOkNSRURFTlRJQUxT");
		}

		// assert
		mockHttp.VerifyNoOutstandingRequest();
		mockHttp.VerifyNoOutstandingExpectation();

		Assert.IsFalse(response.HasErrors);
		Assert.IsNotNull(response.EnabledPaymentInstruments);
		Assert.IsNotNull(response.PaymentMethods);
	}

	[TestMethod]
	public void GoPayClient_Dispose_DisposesHttpClient()
	{
		Assert.Inconclusive();

		// Moq 4.13.1 bug

		//// arrange
		//Mock<HttpMessageHandler> httpMessageHandlerMock = new Mock<HttpMessageHandler>();
		//Mock<HttpClient> httpClientMock = new Mock<HttpClient>(httpMessageHandlerMock.Object);
		//httpClientMock.Protected().Setup("Dispose", true);
		//httpClientMock.Object.BaseAddress = new Uri("https://fake.com");

		//// act
		//using (new GoPayClient(httpClientMock.Object))
		//{
		//	// NOOP
		//}

		//// assert
		//httpClientMock.Protected().Verify("Dispose", Times.Once(), true);
	}

	/* Help methods */

	private GoPayRequest CreatePaymentRequest(string accessToken)
	{
		return new GoPayRequest(accessToken)
		{
			Payer = new GoPayPayer
			{
				DefaultPaymentInstrument = GoPayPaymentInstrument.PAYMENT_CARD,
				AllowedPaymentInstruments = new List<GoPayPaymentInstrument>
				{
					GoPayPaymentInstrument.BANK_ACCOUNT
				},
				DefaultSwift = GoPaySwift.FIOBCZPP,
				AllowedSwifts = new List<GoPaySwift>
				{
					GoPaySwift.FIOBCZPP,
					GoPaySwift.BREXCZPP
				},
				Contact = new GoPayContact
				{
					FirstName = "Test",
					LastName = "Test",
					Email = "devmail@havit.cz",
					PhoneNumber = "+420012345678",
					City = "Prague",
					Street = "Zelený pruh 99",
					PostalCode = "140 00",
					CountryCode = "CZE"
				}
			},
			Target = new GoPayTarget
			{
				Type = GoPayTargetType.Account,
				GoId = 8657786874
			},
			Items = new List<GoPayRequestItem>
			{
				new GoPayRequestItem { Amount = 1, Name = "Test1" },
				new GoPayRequestItem { Amount = 2.5M, Name = "Test2" }
			},
			Currency = GoPayCurrency.CZK,
			OrderNumber = "001",
			OrderDescription = "Předplatné na rok",
			Callback = new GoPayCallback
			{
				NotificationUrl = "http://www.eshop.cz/return",
				ReturnUrl = "http://www.eshop.cz/notify"
			},
			Language = GoPayLanguage.CS
		};
	}

	private GoPayRequest CreatePreauthorizedPaymentRequest(string accessToken)
	{
		GoPayRequest request = CreatePaymentRequest(accessToken);
		request.Preauthorization = true;
		return request;
	}

	private GoPayRequest CreateRecurrentPaymentRequest(string accessToken)
	{
		GoPayRequest request = CreatePaymentRequest(accessToken);
		request.Recurrence = new GoPayRecurrence
		{
			Cycle = GoPayRecurrenceCycle.Day,
			DateTo = DateTime.Now.AddYears(1),
			Period = 7
		};
		return request;
	}

	private GoPayRequest CreateRecurentPaymentOnDemandRequest(string accessToken)
	{
		return new GoPayRequest(accessToken)
		{
			Items = new List<GoPayRequestItem>
			{
				new GoPayRequestItem
				{
					Name = "item01",
					Amount = 5
				}
			},
			Currency = GoPayCurrency.CZK,
			OrderNumber = "001",
			OrderDescription = "Předplatné na rok"
		};
	}

	private object CreateFakePaymentResponse()
	{
		return new
		{
			id = 3000006529,
			order_number = "001",
			state = "CREATED",
			amount = 10,
			currency = "CZK",
			payer = new
			{
				default_payment_instrument = "BANK_ACCOUNT",
				allowed_payment_instruments = new[] { "BANK_ACCOUNT" },
				default_swift = "FIOBCZPP",
				allowed_swifts = new[] { "FIOBCZPP", "BREXCZPP" },
				contact = new
				{
					first_name = "Test",
					last_name = "Test",
					email = "devmail@havit.cz",
					phone_number = "+420123456789",
					city = "Prague",
					street = "Zelený pruh",
					postal_code = "140 00",
					country_code = "CZE"
				}
			},
			target = new
			{
				type = "ACCOUNT",
				goid = 8123456789
			},
			additional_params = new object[] { new { name = "invoicenumber" }, new { value = "2015001003" } },
			lang = "cs",
			gw_url = "https://fake.com/gw/v3/fake"
		};
	}

	private object CreateFakePaymentPreauthorizedResponse()
	{
		return new
		{
			id = 3000006529,
			order_number = "001",
			state = "CREATED",
			amount = 10,
			currency = "CZK",
			payer = new
			{
				default_payment_instrument = "BANK_ACCOUNT",
				allowed_payment_instruments = new[] { "BANK_ACCOUNT" },
				default_swift = "FIOBCZPP",
				allowed_swifts = new[] { "FIOBCZPP", "BREXCZPP" },
				contact = new
				{
					first_name = "Test",
					last_name = "Test",
					email = "devmail@havit.cz",
					phone_number = "+420012345678",
					city = "Prague",
					street = "Zelený pruh 99",
					postal_code = "140 00",
					country_code = "CZE"
				}
			},
			target = new
			{
				type = "ACCOUNT",
				goid = 8123456789
			},
			preauthorization = new
			{
				State = "REQUESTED",
				Requested = true
			},
			additional_params = new object[] { new { name = "invoicenumber" }, new { value = "2015001003" } },
			lang = "cs",
			gw_url = "https://fake.com/gw/v3/fake"
		};
	}

	private object CreateFakeRecurrentPaymentResponse()
	{
		return new
		{
			id = 3000006529,
			order_number = "001",
			state = "CREATED",
			amount = 10,
			currency = "CZK",
			payer = new
			{
				default_payment_instrument = "BANK_ACCOUNT",
				allowed_payment_instruments = new[] { "BANK_ACCOUNT" },
				default_swift = "FIOBCZPP",
				allowed_swifts = new[] { "FIOBCZPP", "BREXCZPP" },
				contact = new
				{
					first_name = "Test",
					last_name = "Test",
					email = "devmail@havit.cz",
					phone_number = "+420123456789",
					city = "Prague",
					street = "Zelený pruh",
					postal_code = "140 00",
					country_code = "CZE"
				}
			},
			target = new
			{
				type = "ACCOUNT",
				goid = 8123456789
			},
			recurrence = new
			{
				recurrence_cycle = "DAY",
				recurrence_period = 7,
				recurrence_date_to = DateTime.Now.AddYears(1).Date.ToString("yyyy-MM-dd"),
				recurrence_state = "REQUESTED"
			},
			additional_params = new object[] { new { name = "invoicenumber" }, new { value = "2015001003" } },
			lang = "cs",
			gw_url = "https://fake.com/gw/v3/fake"
		};
	}

	private object CreateFakeRecurrentOnDemandPaymentResponse()
	{
		return new
		{
			id = 3000006529,
			parent_id = 3000006542,
			order_number = "001",
			state = "CREATED",
			amount = 5,
			currency = "CZK",
			payer = new
			{
				contact = new
				{
					first_name = "Test",
					last_name = "Test",
					email = "devmail@havit.cz",
					phone_number = "+420123456789",
					city = "Prague",
					street = "Zelený pruh",
					postal_code = "140 00",
					country_code = "CZE"
				}
			},
			additional_params = new object[] { new { name = "invoicenumber" }, new { value = "2015001003" } },
			lang = "cs",
			gw_url = "https://fake.com/gw/v3/fake"
		};
	}

	private object CreateFakeAllowedPaymentMethodsResponse()
	{
		return new
		{
			groups = new
			{
				cardPayment = new
				{
					label = new
					{
						cs = "Platební karta"
					}
				},
				bankTransfer = new
				{
					label = new
					{
						cs = "Bankovní platba"
					}
				},
				wallet = new
				{
					label = new
					{
						cs = "Elektronické peněženky"
					}
				},
				others = new
				{
					label = new
					{
						cs = "Ostatní"
					}
				}
			},
			enabledPaymentInstruments = new
			{
				PAYMENT_CARD = new
				{
					label = new
					{
						cs = "Platební karta"
					},
					image = new
					{
						normal = "https://gate.gopay.cz/images/checkout/paymentcard.png",
						large = "https://gate.gopay.cz/images/checkout/payment_card@2x.png"
					},
					currencies = new[] { "CZK", "EUR" },
					group = "card-payment",
					//enabledSwifts = null
				},
				BANK_ACCOUNT = new
				{
					label = new
					{
						cs = "Bankovní platba"
					},
					image = new
					{
						normal = "https://gate.gopay.cz/images/checkout/bankaccount.png",
						large = "https://gate.gopay.cz/images/checkout/bank_account@2x.png"
					},
					currencies = new[] { "CZK", "EUR" },
					group = "bank-transfer",
					enabledSwifts = new
					{
						TATRSKBX = new
						{
							label = new
							{
								cs = "Tatra Banka"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/TATRSKBX.png",
								large = "https://gate.gopay.cz/images/checkout/TATRSKBX@2x.png"
							},
							currencies = new
							{
								EUR = new
								{
									isOnline = true
								}
							}
						},
						GIBACZPX = new
						{
							label = new
							{
								cs = "Česká spořitelna"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/GIBACZPX.png",
								large = "https://gate.gopay.cz/images/checkout/GIBACZPX@2x.png"
							},
							currencies = new
							{
								CZK = new
								{
									isOnline = true
								}
							}
						},
						SUBASKBX = new
						{
							label = new
							{
								cs = "VÚB"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/SUBASKBX.png",
								large = "https://gate.gopay.cz/images/checkout/SUBASKBX@2x.png"
							},
							currencies = new
							{
								EUR = new
								{
									isOnline = true
								}
							}
						},
						KOMBCZPP = new
						{
							label = new
							{
								cs = "eshop.api.swifts.KOMBCZPP"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/KOMBCZPP.png",
								large = "https://gate.gopay.cz/images/checkout/KOMBCZPP@2x.png"
							},
							currencies = new
							{
								CZK = new
								{
									isOnline = true
								}
							}
						},
						UNCRSKBX = new
						{
							label = new
							{
								cs = "UniCredit Bank"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/UNCRSKBX.png",
								large = "https://gate.gopay.cz/images/checkout/UNCRSKBX@2x.png"
							},
							currencies = new
							{
								EUR = new
								{
									isOnline = true
								}
							}
						},
						RZBCCZPP = new
						{
							label = new
							{
								cs = "Raiffeisenbank"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/RZBCCZPP.png",
								large = "https://gate.gopay.cz/images/checkout/RZBCCZPP@2x.png"
							},
							currencies = new
							{
								CZK = new
								{
									isOnline = true
								}
							}
						},
						GIBASKBX = new
						{
							label = new
							{
								cs = "Slovenská spořitelna"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/GIBASKBX.png",
								large = "https://gate.gopay.cz/images/checkout/GIBASKBX@2x.png"
							},
							currencies = new
							{
								EUR = new
								{
									isOnline = true
								}
							}
						},
						CEKOCZPP = new
						{
							label = new
							{
								cs = "ČSOB"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/CEKOCZPP.png",
								large = "https://gate.gopay.cz/images/checkout/CEKOCZPP@2x.png"
							},
							currencies = new
							{
								CZK = new
								{
									isOnline = false
								}
							}
						},
						CEKOSKBX = new
						{
							label = new
							{
								cs = "ČSOB"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/CEKOSKBX.png",
								large = "https://gate.gopay.cz/images/checkout/CEKOSKBX@2x.png"
							},
							currencies = new
							{
								EUR = new
								{
									isOnline = true
								}
							}
						},
						POBNSKBA = new
						{
							label = new
							{
								cs = "Poštová Banka"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/POBNSKBA.png",
								large = "https://gate.gopay.cz/images/checkout/POBNSKBA@2x.png"
							},
							currencies = new
							{
								EUR = new
								{
									isOnline = true
								}
							}
						},
						BREXCZPP = new
						{
							label = new
							{
								cs = "mBank"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/BREXCZPP.png",
								large = "https://gate.gopay.cz/images/checkout/BREXCZPP@2x.png"
							},
							currencies = new
							{
								CZK = new
								{
									isOnline = true
								}
							}
						},
						LUBASKBX = new
						{
							label = new
							{
								cs = "Sberbank"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/LUBASKBX.png",
								large = "https://gate.gopay.cz/images/checkout/LUBASKBX@2x.png"
							},
							currencies = new
							{
								EUR = new
								{
									isOnline = true
								}
							}
						},
						FIOBCZPP = new
						{
							label = new
							{
								cs = "Fio banka"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/FIOBCZPP.png",
								large = "https://gate.gopay.cz/images/checkout/FIOBCZPP@2x.png"
							},
							currencies = new
							{
								CZK = new
								{
									isOnline = true
								}
							}
						},
						OTPVSKBX = new
						{
							label = new
							{
								cs = "OTP Banka"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/OTPVSKBX.png",
								large = "https://gate.gopay.cz/images/checkout/OTPVSKBX@2x.png"
							},
							currencies = new
							{
								EUR = new
								{
									isOnline = true
								}
							}
						},
						RIDBSKBX = new
						{
							label = new
							{
								cs = "ZUNO"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/RIDBSKBX.png",
								large = "https://gate.gopay.cz/images/checkout/RIDBSKBX@2x.png"
							},
							currencies = new
							{
								EUR = new
								{
									isOnline = true
								}
							}
						},
						ZUNOCZPP = new
						{
							label = new
							{
								cs = "ZUNO"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/ZUNOCZPP.png",
								large = "https://gate.gopay.cz/images/checkout/ZUNOCZPP@2x.png"
							},
							currencies = new
							{
								CZK = new
								{
									isOnline = true
								}
							}
						},
						OTHERS = new
						{
							label = new
							{
								cs = "Jiná banka"
							},
							image = new
							{
								normal = "https://gate.gopay.cz/images/checkout/OTHERS.png",
								large = "https://gate.gopay.cz/images/checkout/OTHERS@2x.png"
							},
							currencies = new
							{
								CZK = new
								{
									isOnline = false
								},
								EUR = new
								{
									isOnline = false
								}
							}
						}
					}
				},
				GOPAY = new
				{
					label = new
					{
						cs = "GoPay"
					},
					image = new
					{
						normal = "https://gate.gopay.cz/images/checkout/gopay.png",
						large = "https://gate.gopay.cz/images/checkout/gopay@2x.png"
					},
					currencies = new[] { "CZK", "EUR" },
					group = "wallet",
					//enabledSwifts = null
				},
				BITCOIN = new
				{
					label = new
					{
						cs = "Bitcoin"
					},
					image = new
					{
						normal = "https://gate.gopay.cz/images/checkout/bitcoin.png",
						large = "https://gate.gopay.cz/images/checkout/bitcoin@2x.png"
					},
					currencies = new[] { "CZK", "EUR" },
					group = "wallet",
					//enabledSwifts = null
				},
				PRSMS = new
				{
					label = new
					{
						cs = "Premium SMS"
					},
					image = new
					{
						normal = "https://gate.gopay.cz/images/checkout/prsms.png",
						large = "https://gate.gopay.cz/images/checkout/prsms@2x.png"
					},
					currencies = new[] { "CZK", "EUR" },
					group = "others",
					//enabledSwifts = null
				},
				MPAYMENT = new
				{
					label = new
					{
						cs = "m-platba"
					},
					image = new
					{
						normal = "https://gate.gopay.cz/images/checkout/mpayment.png",
						large = "https://gate.gopay.cz/images/checkout/mpayment@2x.png"
					},
					currencies = new[] { "CZK" },
					group = "others",
					//enabledSwifts = null
				},
				SUPERCASH = new
				{
					label = new
					{
						cs = "superCASH"
					},
					image = new
					{
						normal = "https://gate.gopay.cz/images/checkout/supercash.png",
						large = "https://gate.gopay.cz/images/checkout/supercash@2x.png"
					},
					currencies = new[] { "CZK" },
					group = "others",
					//enabledSwifts = null
				}
			}
		};
	}
}
