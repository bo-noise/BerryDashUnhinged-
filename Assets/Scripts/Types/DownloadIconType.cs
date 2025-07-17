using Newtonsoft.Json;

public class DownloadIconType {
  [JsonProperty("username")]
  public string Username { get; set; }
  [JsonProperty("userid")]
  public string UserID { get; set; }
  [JsonProperty("data")]
  public string Data { get; set; }
  [JsonProperty("uuid")]
  public string UUID { get; set; }
  [JsonProperty("price")]
  public int Price { get; set; }
  [JsonProperty("name")]
  public string Name { get; set; }
}