using System.Numerics;
using Newtonsoft.Json;

public class MarketplaceIconType {
  [JsonProperty("data")]
  public string Data { get; set; }
  [JsonProperty("uuid")]
  public string UUID { get; set; }
  [JsonProperty("price")]
  public BigInteger Price { get; set; }
  [JsonProperty("name")]
  public string Name { get; set; }
}