using Newtonsoft.Json;

public class MarketplaceIconType {
  [JsonProperty("username")]
  public string CreatorUsername { get; set; }
  [JsonProperty("userid")]
  public string CreatorUserID { get; set; }
  [JsonProperty("data")]
  public string Data { get; set; }
  [JsonProperty("uuid")]
  public string UUID { get; set; }
  [JsonProperty("price")]
  public int Price { get; set; }
  [JsonProperty("name")]
  public string Name { get; set; }
}