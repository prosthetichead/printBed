using System.ComponentModel.DataAnnotations;

namespace PrintBed.Models
{
    public class PrintDetailPage
    {
        public Print Print { get; set; }
        public List<PrintFile> Files { get; set; }
        public int totalPages { get; set; } = 1;
        public int currentPage { get; set; } = 1;
    }

    public class Category
    {
        [Key]
        public string Id { get; set; } = "";
        public string? Name { get; set; }
        public string? ImagePath { get; set; }
        public ICollection<Print> Prints { get; } = new List<Print>();
    }

    public class Creator
    {
        [Key]
        public string Id { get; set; } = "";
        public string? Name { get; set; }
        public string? ImagePath { get; set; }
        public ICollection<Print> Prints { get; } = new List<Print>();
    }

    public class Print
    {
        [Key]
        public string Id { get; set; } = "";
        public string Name { get; set; } = "Untitled";
        public string? Description { get; set; } = null;

        [Display(Name = "Category")]
        public string CategoryId { get; set; }
        public Category? Category { get; set; }

        [Display(Name = "Creator")]
        public string CreatorId { get; set; }
        public Creator? Creator { get; set; }

        [Display(Name = "Print Instructions")]
        public string? PrintInstructions { get; set; }      
        
        public ICollection<PrintFile> PrintFiles { get; } = new List<PrintFile>();
    }

    public class PrintFile
    {
        [Key]
        public string Id { get; set; } = "";
        public string DisplayName { get; set; } = "untitled";
        public string? Description { get; set; }
        public string FileName { get; set; } = "untitled.bla";
        public string FileExtension { get; set; } = "bla";
        public string FilePath { get; set; } = "";
        public double FileSize { get; set; } = 0;
        public string? FileTypeId { get; set; }
        public FileType? FileType { get; set; }
        public string? PrintId { get; set; }
        public Print? Print { get; set; }
    }
    public class FileType
    {
        [Key]
        public string Id { get; set; } = "";
        public string? Extensions {  get; set; } //comma seperated string of extensions this type may have
        public string? Name { get; set; }
        public string? ImagePath { get; set; }
        public string PreviewType { get; set; }
        public ICollection<PrintFile> PrintFiles { get; } = new List<PrintFile>();
    }
}
