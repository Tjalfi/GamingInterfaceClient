using System.Collections.Generic;
using Newtonsoft.Json;

namespace GamingInterfaceClient.Models
{
    public class Command
    {
        [JsonProperty(PropertyName = "Key")]
        private string key = "a";

        [JsonProperty(PropertyName = "Modifier")]
        private List<string> modifiers = new List<string>();

        [JsonProperty(PropertyName = "ActivatorType")]
        private int activatorType;

        public void AddModifier(string modifier)
        {
            modifiers.Add(modifier);
        }

        public string GetKey()
        {
            return key;
        }

        public void SetKey(string key)
        {
            this.key = key;
        }

        public List<string> GetModifiers()
        {
            return modifiers;
        }

        public int GetActivatorType()
        {
            return activatorType;
        }

        public void SetActivatorType(int activatorType)
        {
            this.activatorType = activatorType;
        }

        public void RemoveAllModifiers()
        {
            modifiers = new List<string>();
        }
    }
}
