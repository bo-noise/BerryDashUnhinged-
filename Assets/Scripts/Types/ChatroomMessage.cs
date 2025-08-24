using UnityEngine.Scripting;
using Newtonsoft.Json;
using System.Numerics;
using Newtonsoft.Json.Linq;

[Preserve]
public class ChatroomMessage {
  [Preserve]
  [JsonProperty("username")]
  public string Username { get; set; }

  [Preserve]
  [JsonProperty("userid")]
  public BigInteger UserID { get; set; }

  [Preserve]
  [JsonProperty("content")]
  public string Content { get; set; }

  [Preserve]
  [JsonProperty("id")]
  public BigInteger ID { get; set; }

  [Preserve]
  [JsonProperty("icon")]
  public int Icon { get; set; }

  [Preserve]
  [JsonProperty("overlay")]
  public int Overlay { get; set; }

  [Preserve]
  [JsonProperty("birdColor")]
  public int[] BirdColor { get; set; }

  [Preserve]
  [JsonProperty("overlayColor")]
  public int[] OverlayColor { get; set; }
}