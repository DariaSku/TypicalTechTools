using Humanizer.Localisation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TypicalTechTools.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        [Display(Name = "Comment")]
        public string CommentText { get; set; } = string.Empty;

        //make our foreign keys
        [Display(Name = "Product Code")]
        public int ProductId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }  // Дата создания комментария

        //  идентификатор сессии пользователя, чтобы дать возможность редактировать комментарий в ограниченный период времени
        public string? SessionId { get; set; } = string.Empty;  // Связь комментария с сессией пользователя

        //mapping out realtionship in the ORM, this is the reference from the 'Many' side of the realtionship
        // [ForeignKey("ProductCode")]
        public virtual Product? Product { get; set; }

       public string ToCSVString()
       {
           return $"{CommentId},{CommentText},{ProductId}";
       }

        // Новая функция: проверка прав на редактирование
        public bool CanEdit(string currentSessionId)
        {
            return SessionId == currentSessionId && DateTime.Now <= CreatedDate.AddMinutes(1);
        }
    }
}
