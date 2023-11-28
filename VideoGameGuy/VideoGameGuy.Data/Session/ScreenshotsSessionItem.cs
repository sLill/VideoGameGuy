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

            // Start writing the JSON
            writer.WriteStartObject();

            // Serialize GameTitle
            writer.WritePropertyName("GameTitle");
            serializer.Serialize(writer, screenshotsRound.GameTitle);

            // Serialize ImageCollection
            writer.WritePropertyName("ImageCollection");
            writer.WriteStartArray();
            foreach (var image in screenshotsRound.ImageCollection)
            {
                serializer.Serialize(writer, image);
            }
            writer.WriteEndArray();

            // Serialize IsSolved
            writer.WritePropertyName("IsSolved");
            serializer.Serialize(writer, screenshotsRound.IsSolved);

            // Serialize IsSkipped
            writer.WritePropertyName("IsSkipped");
            serializer.Serialize(writer, screenshotsRound.IsSkipped);

            // End writing the JSON
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Ensure the token type is correct (StartObject indicates the beginning of an object)
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException("Expected StartObject token.");

            var screenshotsRound = new ScreenshotsRound();
            reader.Read(); // Read the next token

            while (reader.TokenType != JsonToken.EndObject) // Read until the end of the object
            {
                // Make sure the token is a property name
                if (reader.TokenType != JsonToken.PropertyName)
                    throw new JsonSerializationException("Expected PropertyName token.");

                string propertyName = reader.Value.ToString();
                reader.Read(); // Move to the property value

                switch (propertyName)
                {
                    case "GameTitle":
                        screenshotsRound.GameTitle = serializer.Deserialize<string>(reader);
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

                reader.Read(); // Read the next token (either the next property name or the end object)
            }

            return screenshotsRound;
        }

        public override bool CanConvert(Type objectType)
            => objectType == typeof(ScreenshotsRound);
    }
}
