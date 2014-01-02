using System;
using System.ComponentModel.DataAnnotations;

namespace RM.Unify.Sdk.SampleApp2.Models
{
    public class Post
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
        public DateTime Created { get; set; }
        public int AccountId { get; set; }
        public virtual Account Account { get; set; }
    }
}