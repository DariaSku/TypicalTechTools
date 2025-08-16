using TypicalTechTools.Models;
using TypicalTechTools.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;

namespace TypicalTechTools.Models.Repositories
{
    /// <summary>
    /// Repository for managing product comments in the system.
    /// </summary>
    /// <remarks>
    /// Provides methods to create, retrieve, update, and delete comments.
    /// Comments are linked to products and support session-based editing permissions.
    /// </remarks>
    public class CommentRepository : ICommentRepository
    {
        private readonly TypicalTechToolsDBContext _context;
        public CommentRepository(TypicalTechToolsDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all comments associated with a specific product.
        /// </summary>
        /// <param name="productId">The ID of the product</param>
        /// <returns>List of comments for the product</returns>
        public List<Comment> GetCommentsByProductId(int productId)
        {
            return _context.Comments
                           .Where(c => c.ProductId == productId)
                           .Include(c => c.Product) 
                           .ToList();
        }

        /// <summary>
        /// Adds a new comment to the database.
        /// </summary>
        /// <param name="comment">Comment to be created</param>
        public void CreateComment(Comment comment)
        {
            _context.Comments.Add(comment);
            _context.SaveChanges();
        }

        /// <summary>
        /// Retrieves all comments in the system, including related product data.
        /// </summary>
        /// <returns>List of all comments</returns>
        public List<Comment> GetAllComments()
        {
            return _context.Comments
               .Include(c => c.Product) 
               .ToList();
        }

        /// <summary>
        /// Retrieves a single comment by its ID.
        /// </summary>
        /// <param name="id">Comment ID</param>
        /// <returns>Comment object or null if not found</returns>
        public Comment GetCommentById(int id)
        {
            var comment = _context.Comments
                      .Include(c => c.Product)
                      .FirstOrDefault(c => c.CommentId == id);
            return comment;
        }

        /// <summary>
        /// Updates an existing comment.
        /// </summary>
        /// <param name="comment">Modified comment object</param>
        public void UpdateComment(Comment comment)
        {
          //  var comment = GetCommentById(id);

            _context.Comments.Update(comment);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletes a comment from the database by its ID.
        /// </summary>
        /// <param name="id">ID of the comment to delete</param>
        public void DeleteComment(int id)
        {
            var comment = GetCommentById(id);
            _context.Comments.Remove(comment);
            _context.SaveChanges();
        }
    }
}
