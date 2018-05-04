using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Data;
using System.Net.NetworkInformation;
using Wox.Plugin;
using Newtonsoft.Json;

public class MovieSearch
{
    public DataTable Search { get; set; }
    public int totalResults { get; set; }
    public bool Response { get; set; }
    public string Error { get; set; }
}

public class Movie
{
    public string Title { get; set; }
    public string Year { get; set; }
    public string Rated { get; set; }
    public string Released { get; set; }
    public string Runtime { get; set; }
    public string Genre { get; set; }
    public string Director { get; set; }
    public string Writer { get; set; }
    public string Actors { get; set; }
    public string Plot { get; set; }
    public string Language { get; set; }
    public string Country { get; set; }
    public string Awards { get; set; }
    public string Poster { get; set; }
    public DataTable Ratings { get; set; }
    public string Metascore { get; set; }
    public string imdbRating { get; set; }
    public string imdbVotes { get; set; }
    public string imdbID { get; set; }
    public string Type { get; set; }
    public string DVD { get; set; }
    public string BoxOffice { get; set; }
    public string Production { get; set; }
    public string Website { get; set; }
    public string Resposnse { get; set; } //can't be read as bool for some reason
}

namespace Wox.Plugin.Test
{
    public class Program : IPlugin
    {
        
        public void Init(PluginInitContext context)
        {
           
        }

      

        public List<Result> Query(Query query)
        {
            List<Result> results = new List<Result>();

            String defaulticon = "image.png";
            var queryString = query.Search;
            if(string.IsNullOrWhiteSpace(queryString) || queryString.Length <= 1)
            {
                results.Add(Result("Enter Movie to Search for:", "", defaulticon, Action("null")));
                return results;
            }

            


            var movieSearchJSON = new WebClient().DownloadString("http://www.omdbapi.com/?s=" + queryString + "&apikey=b57d40cf");
            MovieSearch movieSearch = JsonConvert.DeserializeObject<MovieSearch>(movieSearchJSON);
            if(!movieSearch.Response)
            {
                results.Add(Result("No Title found.", "No Title found.", defaulticon, Action("null")));
                return results;
            }

            foreach (DataRow row in movieSearch.Search.Rows)
            {
                var movieJSON = new WebClient().DownloadString("http://www.omdbapi.com/?i=" + row["imdbID"] + "&apikey=b57d40cf");
                Movie movie = JsonConvert.DeserializeObject<Movie>(movieJSON);
                if (movie.Resposnse == "false")
                {
                    continue;
                }
                results.Add(Result(movie.Title + " by " + movie.Director + " | " + movie.Actors, movie.Plot, defaulticon, Action(movie.imdbID)));

                
                

            }
            return results;
        }
        private static Result Result(String title, String subtitle, String icon, Func<ActionContext, bool> action)
        {
            return new Result()
            {
                Title = title,
                SubTitle = subtitle,
                IcoPath = icon,
                Action = action
            };
        }

        // The Action method is called after the user selects the item
        private static Func<ActionContext, bool> Action(String text)
        {
            return e =>
            {
                if(text.StartsWith("tt"))
                {
                    System.Diagnostics.Process.Start("https://www.imdb.com/title/"+text);
                }
                    // return false to tell Wox don't hide query window, otherwise Wox will hide it automatically
                    return false;
            };
        }

        
    }
}