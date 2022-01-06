using Azure;
using Azure.AI.TextAnalytics;
using System.Data.SQLite;
using System.Configuration;

namespace Example
{
    class Program
    {
    
       // private readonly AzureKeyCredential credentials = new AzureKeyCredential(ConfigurationManager.AppSettings.Get("AzureKey"));
        

        private static readonly Uri endpoint = new Uri(ConfigurationManager.AppSettings.Get("endpoint"));

        // Example method for detecting sentiment from text 
        //static void SentimentAnalysisExample(TextAnalyticsClient client)
        //{

        //    string inputText = "I had the best day of my life. I wish you were there with me.";
        //    DocumentSentiment documentSentiment = client.AnalyzeSentiment(inputText);
        //    Console.WriteLine($"Document sentiment: {documentSentiment.Sentiment}\n");
            
        //    foreach (var sentence in documentSentiment.Sentences)
        //    {
        //        Console.WriteLine($"\tText: \"{sentence.Text}\"");
        //        Console.WriteLine($"\tSentence sentiment: {sentence.Sentiment}");
        //        Console.WriteLine($"\tPositive score: {sentence.ConfidenceScores.Positive:0.00}");
        //        Console.WriteLine($"\tNegative score: {sentence.ConfidenceScores.Negative:0.00}");
        //        Console.WriteLine($"\tNeutral score: {sentence.ConfidenceScores.Neutral:0.00}\n");
        //    }
        //}

        static (double, double, double) SentimentAnalysisExample(TextAnalyticsClient client, string input)
        {

            double a = 0;
            double b = 0;
            double c = 0;

            double d = 0;
            double e = 0;
            double f = 0;

            // string inputText = "I had the best day of my life. I wish you were there with me.";
            DocumentSentiment documentSentiment = client.AnalyzeSentiment(input);
            Console.WriteLine($"Document sentiment: {documentSentiment.Sentiment}\n");
            string stuff = documentSentiment.Sentiment.ToString();
            //  Console.WriteLine(stuff);


            if (stuff.Equals("Positive"))
            {
                d++;
            } else if (stuff.Equals("Negative"))
            {
                e++;
            } else if (stuff.Equals("Neutral"))
            {
                f++;
            }
           


            foreach (var sentence in documentSentiment.Sentences)
            {
                Console.WriteLine($"\tText: \"{sentence.Text}\"");
                Console.WriteLine($"\tSentence sentiment: {sentence.Sentiment}");
                Console.WriteLine($"\tPositive score: {sentence.ConfidenceScores.Positive:0.00}");
                Console.WriteLine($"\tNegative score: {sentence.ConfidenceScores.Negative:0.00}");
                Console.WriteLine($"\tNeutral score: {sentence.ConfidenceScores.Neutral:0.00}\n");
                //a = sentence.ConfidenceScores.Positive;
              //  b = sentence.ConfidenceScores.Negative;
              //  c = sentence.ConfidenceScores.Neutral;
               
            }
            return (d, e, f);
           }

        static SQLiteConnection CreateConnection()
        {
            string connectionString = ConfigurationManager.AppSettings.Get("DbConnectionString");
            SQLiteConnection sqlite_conn;
            sqlite_conn = new SQLiteConnection(connectionString);
         try
            {
                sqlite_conn.Open();
                Console.WriteLine("opened");
            }
            catch (Exception ex)
            {
                Console.WriteLine("not open");
            }
            return sqlite_conn;
        }
        static void ReadData(SQLiteConnection conn, string numbers)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            
            sqlite_cmd.CommandText = "select text from message as m left join handle as h on '" + numbers + "'= h.id where m.service = h.service";
            Console.WriteLine(sqlite_cmd.CommandText);
            double pAverage = 0;
            double nAverage = 0;
            double aAverage = 0;
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
               string  myreader = sqlite_datareader.GetString(0);
               AzureKeyCredential? credential = new AzureKeyCredential(ConfigurationManager.AppSettings.Get("AzureKey"));
                var client = new TextAnalyticsClient(endpoint, credential);
                var values = SentimentAnalysisExample(client, myreader);
                pAverage += values.Item1;
                nAverage += values.Item2;
                aAverage += values.Item3;
                //times++;
                Console.WriteLine("positive: " + pAverage);
                Console.WriteLine("negative: " + nAverage);
                Console.WriteLine("meh: "  + aAverage);
;               

            }
            //pAverage = pAverage / times;
            //nAverage = nAverage / times;
           // aAverage = pAverage / times;

            Console.WriteLine("Positive = " + pAverage);
            Console.WriteLine("Negative = " + nAverage);
            Console.WriteLine("Neutral  = " + aAverage);
            conn.Close();
        }



        // Example method for detecting opinions text 
        static void SentimentAnalysisWithOpinionMiningExample(TextAnalyticsClient client)
        {
            var documents = new List<string>
            {
                "The food and service were unacceptable, but the concierge were nice.",
               
            };

            AnalyzeSentimentResultCollection reviews = client.AnalyzeSentimentBatch(documents, options: new AnalyzeSentimentOptions()
            {
                IncludeOpinionMining = true
            });

            foreach (AnalyzeSentimentResult review in reviews)
            {
                Console.WriteLine($"Document sentiment: {review.DocumentSentiment.Sentiment}\n");
                Console.WriteLine($"\tPositive score: {review.DocumentSentiment.ConfidenceScores.Positive:0.00}");
                Console.WriteLine($"\tNegative score: {review.DocumentSentiment.ConfidenceScores.Negative:0.00}");
                Console.WriteLine($"\tNeutral score: {review.DocumentSentiment.ConfidenceScores.Neutral:0.00}\n");
                foreach (SentenceSentiment sentence in review.DocumentSentiment.Sentences)
                {
                    Console.WriteLine($"\tText: \"{sentence.Text}\"");
                    Console.WriteLine($"\tSentence sentiment: {sentence.Sentiment}");
                    Console.WriteLine($"\tSentence positive score: {sentence.ConfidenceScores.Positive:0.00}");
                    Console.WriteLine($"\tSentence negative score: {sentence.ConfidenceScores.Negative:0.00}");
                    Console.WriteLine($"\tSentence neutral score: {sentence.ConfidenceScores.Neutral:0.00}\n");

                    foreach (SentenceOpinion sentenceOpinion in sentence.Opinions)
                    {
                        Console.WriteLine($"\tTarget: {sentenceOpinion.Target.Text}, Value: {sentenceOpinion.Target.Sentiment}");
                        Console.WriteLine($"\tTarget positive score: {sentenceOpinion.Target.ConfidenceScores.Positive:0.00}");
                        Console.WriteLine($"\tTarget negative score: {sentenceOpinion.Target.ConfidenceScores.Negative:0.00}");
                        foreach (AssessmentSentiment assessment in sentenceOpinion.Assessments)
                        {
                            Console.WriteLine($"\t\tRelated Assessment: {assessment.Text}, Value: {assessment.Sentiment}");
                            Console.WriteLine($"\t\tRelated Assessment positive score: {assessment.ConfidenceScores.Positive:0.00}");
                            Console.WriteLine($"\t\tRelated Assessment negative score: {assessment.ConfidenceScores.Negative:0.00}");
                        }
                    }
                }
                Console.WriteLine($"\n");
            }
        }

        static void Main(string[] args)
        {

            AzureKeyCredential? credential = new AzureKeyCredential(ConfigurationManager.AppSettings.Get("AzureKey"));
            var client = new TextAnalyticsClient(endpoint, credential);
            // SentimentAnalysisExample(client);
            //  SentimentAnalysisWithOpinionMiningExample(client);
            Console.WriteLine("Enter number of person who you want to analyze texts with");
            string number = Console.ReadLine();
            number = "+1" + number;
            Console.WriteLine(number);
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();
            ReadData(sqlite_conn, number);

            Console.Write("Press any key to exit.");
            Console.ReadKey();
        }

    }
}