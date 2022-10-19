using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace MoralisUnity.Samples.Shared.UnityWeb3Tools.Models
{
    [DataContract]
    public class NftMetadata
    {
        [DataMember(Name = "name", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        
        [DataMember(Name = "description", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        
        [DataMember(Name = "image", EmitDefaultValue = false)]
        [JsonProperty(PropertyName = "image")]
        public string ImageUrl { get; set; }
    }   
}
