using Newtonsoft.Json;
using static VideoGameGuy.Data.ScreenshotsSessionItem;

namespace VideoGameGuy.Data
{
    public class ScreenshotsSessionItem : SessionItemBase
    {
        #region Records..
        public record ImageRecord
        {
            public string Value { get; set; }
        }

        [JsonConverter(typeof(ScreenshotRoundConverter))]
        public record ScreenshotsRound
        {
            public string GameTitle { get; set; }
            public string GameSlug { get; set; }
            public List<ImageRecord> ImageCollection { get; set; }
            public bool IsSolved { get; set; }
            public bool IsSkipped { get; set; }
        }
        #endregion Records..

        #region Properties..

        public List<ScreenshotsRound> ScreenshotsRounds { get; set; } = new List<ScreenshotsRound>();

        public int HighestScore { get; set; }

        public int CurrentScore
            => ScreenshotsRounds.Count(x => x.IsSolved);

        public ScreenshotsRound CurrentRound
            => ScreenshotsRounds.LastOrDefault(x => !x.IsSolved && !x.IsSkipped);
        #endregion Properties..
    }

    public class ScreenshotRoundConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var screenshotsRound = value as ScreenshotsRound;
            if (screenshotsRound == null)
                throw new ArgumentException("Expected ScreenshotsRound object value");

            writer.WriteStartObject();

            writer.WritePropertyName("GameTitle");
            serializer.Serialize(writer, screenshotsRound.GameTitle);

            writer.WritePropertyName("GameSlug");
            serializer.Serialize(writer, screenshotsRound.GameSlug);

            writer.WritePropertyName("ImageCollection");
            writer.WriteStartArray();

            foreach (var image in screenshotsRound.ImageCollection)
                serializer.Serialize(writer, image);

            writer.WriteEndArray();

            writer.WritePropertyName("IsSolved");
            serializer.Serialize(writer, screenshotsRound.IsSolved);

            writer.WritePropertyName("IsSkipped");
            serializer.Serialize(writer, screenshotsRound.IsSkipped);

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException("Expected StartObject token.");

            var screenshotsRound = new ScreenshotsRound();
            reader.Read(); 

            while (reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    throw new JsonSerializationException("Expected PropertyName token.");

                string propertyName = reader.Value.ToString();
                reader.Read();

                switch (propertyName)
                {
                    case "GameTitle":
                        screenshotsRound.GameTitle = serializer.Deserialize<string>(reader);
                        break;
                    case "GameSlug":
                        screenshotsRound.GameSlug = serializer.Deserialize<string>(reader);
                        break;
                    case "ImageCollection":
                        screenshotsRound.ImageCollection = serializer.Deserialize<List<ImageRecord>>(reader);
                        break;
                    case "IsSolved":
                        screenshotsRound.IsSolved = serializer.Deserialize<bool>(reader);
                        break;
                    case "IsSkipped":
                        screenshotsRound.IsSkipped = serializer.Deserialize<bool>(reader);
                        break;
                    default:
                        throw new JsonSerializationException($"Unexpected property: {propertyName}");
                }

                reader.Read(); 
            }

            return screenshotsRound;
        }

        public override bool CanConvert(Type objectType)
            => objectType == typeof(ScreenshotsRound);
    }
}
