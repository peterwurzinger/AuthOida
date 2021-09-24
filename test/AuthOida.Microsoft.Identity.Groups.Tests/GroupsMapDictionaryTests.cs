using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace AuthOida.Microsoft.Identity.Groups.Tests
{
    public class GroupsMapDictionaryTests
    {
        [Fact]
        public void CtorThrowsIfBackingDictionaryIsNull()
        {
            Assert.Throws<ArgumentNullException>("backingDictionary", () => new GroupsMapDictionary(null!));
        }

        [Fact]
        public void CtorShouldCopyDictionary()
        {
            var dictionary = new Dictionary<string, string>
            {
                {"1234", "Testgroup" }
            };

            var groupsMap = new GroupsMapDictionary(dictionary);
            dictionary["1234"] = "AlteredGroup";

            Assert.Equal("Testgroup", groupsMap["1234"]);
        }

        [Fact]
        public void ShouldBehaveLikeDictionary()
        {
            var dictionary = new Dictionary<string, string>
            {
                {"1234", "Testgroup" },
                {"5678", "AnotherGroup" }
            };
            var groupsMap = new GroupsMapDictionary(dictionary);

            Assert.Equal(dictionary.TryGetValue("1234", out var valueFromDictionary), groupsMap.TryGetValue("1234", out var valueFromGroupsMap));
            Assert.Equal(valueFromDictionary, valueFromGroupsMap);
            Assert.Equal(dictionary.Keys, groupsMap.Keys);
            Assert.Equal(dictionary.Values, groupsMap.Values);
            Assert.Equal(dictionary.Count, groupsMap.Count);
            Assert.Equal(dictionary.ContainsKey("1234"), groupsMap.ContainsKey("1234"));
            Assert.NotStrictEqual(dictionary.GetEnumerator(), groupsMap.GetEnumerator());
            Assert.NotStrictEqual(((IEnumerable)dictionary).GetEnumerator(), ((IEnumerable)groupsMap).GetEnumerator());
        }
    }
}
