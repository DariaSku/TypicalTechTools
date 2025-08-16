

namespace TypicalTechTools.Models.Repositories
{
    public interface ICommentRepository
    {
        List<Comment> GetCommentsByProductId(int productId);
        List<Comment> GetAllComments();
        Comment GetCommentById(int id);
        void CreateComment(Comment comment);
        void UpdateComment(Comment comment);
        void DeleteComment(int id);
    }
}
