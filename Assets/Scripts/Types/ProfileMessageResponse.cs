using UnityEngine.Scripting;
using Newtonsoft.Json;
using System.Numerics;

[Preserve]
public class ProfileMessageResponse  {
  [Preserve]
  [JsonProperty("id")]
  public BigInteger ID { get; set; }

  [Preserve]
  [JsonProperty("userId")]
  public BigInteger UserID { get; set; }

  [Preserve]
  [JsonProperty("content")]
  public string Content { get; set; }

  [Preserve]
  [JsonProperty("timestamp")]
  public string Timestamp { get; set; }

  [Preserve]
  [JsonProperty("likes")]
  public BigInteger Likes { get; set; }
}