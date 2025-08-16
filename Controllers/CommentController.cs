using TypicalTechTools.DataAccess;
using TypicalTechTools.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TypicalTechTools.Models.Repositories;
using System.Xml.Linq;
using NuGet.Protocol.Core.Types;
using Ganss.Xss;

namespace TypicalTools.Controllers
{
    public class CommentController : Controller
    {
        HtmlSanitizer sanitizer = new HtmlSanitizer();

        private readonly ICommentRepository _commentRepository;
        private readonly HtmlSanitizer _sanitizer;

        public CommentController(ICommentRepository commentRepository, HtmlSanitizer sanitizer)
        {
            _commentRepository = commentRepository;
            _sanitizer = sanitizer;
        }

        /// <summary>
        /// Displays all comments across all products.
        /// </summary>
        /// <returns>List of all comments.</returns>
        // GET: CommentController
        public IActionResult Index()
        {
            var comments = _commentRepository.GetAllComments();
            return View(comments);
        }

        /// <summary>
        /// Displays comments for a specific product.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Comment list view for the specified product.</returns>
        // GET: CommentController/CommentList/5
        public IActionResult CommentList(int id)
        {
            var comments = _commentRepository.GetCommentsByProductId(id);

            // Не перенаправляем, даже если комментариев нет
            ViewBag.ProductId = id; // чтобы сохранить ID продукта в представлении
            return View("CommentList", comments);
        }

        /// <summary>
        /// Displays the form to create a new comment for a product.
        /// </summary>
        /// <param name="productId">ID of the product being commented on</param>
        /// <returns>Comment creation form with pre-filled data.</returns>
        // GET:  CommentController/Create
        public ActionResult Create(int productId)
        {
            var comment = new Comment
            {
                ProductId = productId,
                CreatedDate = DateTime.Now,
                SessionId = HttpContext.Session.Id
            };
            return View(comment);
        }

        /// <summary>
        /// Handles submission of a new comment.
        /// Sanitizes input and attaches session ID and timestamp.
        /// </summary>
        /// <param name="comment">Comment data from form</param>
        /// <returns>Redirects to comment list or reloads form if invalid.</returns>
        // POST: CommentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                // This ensures that the session ID stays fixed and doesn't reset on each request
                HttpContext.Session.SetString("SessionId", HttpContext.Session.Id);
                // Assign the current session ID to the comment record
                comment.SessionId = HttpContext.Session.Id;
                comment.CreatedDate = DateTime.Now;

                // Санитизируй обновлённый текст
                comment.CommentText = _sanitizer.Sanitize(comment.CommentText);

                _commentRepository.CreateComment(comment);
                return RedirectToAction("CommentList", new { id = comment.ProductId });
            }
            return View(comment);
        }

        /// <summary>
        /// Displays the edit form for a comment.
        /// </summary>
        /// <param name="id">ID of the comment to edit</param>
        /// <returns>Edit view or redirect if access denied (non-admin).</returns>
        // GET: CommentController/Edit/5
        public ActionResult Edit(int id)
        {
            var comment = _commentRepository.GetCommentById(id);
            bool isAdmin = User.IsInRole("ADMIN");

            // Только если не админ — проверка времени и сессии
            if (!isAdmin &&
                (comment.SessionId != HttpContext.Session.Id || comment.CreatedDate.AddMinutes(10) < DateTime.Now))
            {
                TempData["Message"] = "Sorry, you can no longer DELETE this comment. The allowed time has expired.";
                return RedirectToAction("CommentList", new { id = comment.ProductId });
            }
            return View(comment);
        }

        /// <summary>
        /// Handles update to a comment after edit.
        /// Checks time and session ID for non-admins.
        /// </summary>
        /// <param name="comment">Updated comment data</param>
        /// <returns>Redirects to comment list or reloads form on error.</returns>
        // POST: CommentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Comment comment)
        {
            try
            {
                bool isAdmin = User.IsInRole("ADMIN");
                var existingComment = _commentRepository.GetCommentById(comment.CommentId);

                if (!isAdmin &&
                    (existingComment.SessionId != HttpContext.Session.Id || existingComment.CreatedDate.AddMinutes(1) < DateTime.Now))
                {
                    TempData["Message"] = "Sorry, you can no longer EDIT this comment. The allowed time has expired.";
                    return RedirectToAction("CommentList", new { id = existingComment.ProductId });
                }

                existingComment.CommentText = _sanitizer.Sanitize(comment.CommentText);
                _commentRepository.UpdateComment(existingComment);

                return RedirectToAction("CommentList", new { id = existingComment.ProductId });
            }
            catch
            {
                return View(comment);
            }
        }

        /// <summary>
        /// Displays the confirmation view to delete a comment.
        /// </summary>
        /// <param name="id">ID of the comment to delete</param>
        /// <returns>Delete confirmation view or redirect if access denied.</returns>
        // GET: CommentController/Delete/5
        public ActionResult Delete(int id)
        {
            var comment = _commentRepository.GetCommentById(id);
            bool isAdmin = User.IsInRole("ADMIN");

            if (!isAdmin &&
                (comment.SessionId != HttpContext.Session.Id || comment.CreatedDate.AddMinutes(10) < DateTime.Now))
            {
                TempData["Message"] = "Sorry, you can no longer DELETE this comment. The allowed time has expired.";
                return RedirectToAction("CommentList", new { id = comment.ProductId });
            }

            return View(comment);
        }

        /// <summary>
        /// Handles actual deletion of a comment.
        /// </summary>
        /// <param name="id">ID of the comment to delete</param>
        /// <param name="collection">Form data (not used)</param>
        /// <returns>Redirects to comment list or reloads view on error.</returns>
        // POST: CommentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                bool isAdmin = User.IsInRole("ADMIN");
                var comment = _commentRepository.GetCommentById(id);

                if (!isAdmin &&
                    (comment.SessionId != HttpContext.Session.Id || comment.CreatedDate.AddMinutes(10) < DateTime.Now))
                {
                    TempData["Message"] = "Sorry, you can no longer DELETE this comment. The allowed time has expired.";
                    return RedirectToAction("CommentList", new { id = comment.ProductId });
                }

                _commentRepository.DeleteComment(id);
                return RedirectToAction("CommentList", new { id = comment.ProductId });
            }
            catch
            {
                var comment = _commentRepository.GetCommentById(id);
                return View(comment);
            }
        }
    }
}
