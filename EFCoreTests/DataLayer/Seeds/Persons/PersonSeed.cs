using Havit.Data.Patterns.DataSeeds;
using Havit.EFCoreTests.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.EFCoreTests.DataLayer.Seeds.Persons
{
    public class PersonSeed : DataSeed<PersonsProfile>
    {
        public override void SeedData()
        {
            var persons = CreatePersons(200).ToArray();
            foreach (Person person in persons)
            {
                person.Subordinates.AddRange(CreatePersons(3));
                foreach (Person subordinate in person.Subordinates)
                {
                    subordinate.Subordinates.AddRange(CreatePersons(3));
                }
            }
            Seed(For(persons).PairBy(p => p.Name));
        }

        private List<Person> CreatePersons(int count)
        {
            return Enumerable.Range(0, count).Select(i => new Person()).ToList();
        }
    }
}
