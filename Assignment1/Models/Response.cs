using System.Collections.Generic;

namespace Admin.Models
{
    public class Response
    {
        // response starts with code
        public int statusCode {  get; set; }

        public string messageCode {  get; set; }

        public Produce produce{ get; set; }

        public List<Produce> produces { get; set;}
    }
}
