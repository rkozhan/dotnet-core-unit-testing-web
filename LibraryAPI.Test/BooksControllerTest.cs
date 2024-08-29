using Library.API.Controllers;
using Library.API.Data.Models;
using Library.API.Data.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Xunit;

namespace LibraryAPI.Test
{
    public class BooksControllerTest
    {
        BooksController _controller;
        IBookService _service;

        public BooksControllerTest() 
        {
            _service = new BookService();
            _controller = new BooksController(_service);
        }

        [Fact]
        public void GetAllTest ()
        {
            //arrange
            //act
            var result = _controller.Get();
            //assert
            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<Book>>(list.Value);

            var lisrBooks = list.Value as List<Book>;

            Assert.Equal(5, lisrBooks.Count);
        }

        [Theory]
        [InlineData("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200", "ab2bd817-98cd-4cf3-a80a-53ea0cd9c111")]
        public void GetBookByIdTest(string guid1, string guid2)
        {
            //arrange
            var validGuid = new Guid(guid1);
            var invalidGuid = new Guid(guid2);

            //act
            var notFoundResult = _controller.Get(invalidGuid);
            var okREsult = _controller.Get(validGuid);

            //assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);

            Assert.IsType<OkObjectResult>(okREsult.Result);

            var item = okREsult.Result as OkObjectResult;
            Assert.IsType<Book>(item.Value);

            var bookItem = item.Value as Book;
            Assert.Equal(validGuid, bookItem.Id);
            //title
            Assert.Equal("Managing Oneself", bookItem.Title);
        }

        [Fact]
        public void AddBookTest()
        {
            //arrange
            var completeBook = new Book()
            {
                Author = "Author",
                Title = "Title",
                Description = "Description"
            };
            //act
            var createdResponce = _controller.Post(completeBook);
            //assert
            Assert.IsType<CreatedAtActionResult>(createdResponce);

            var item = createdResponce as CreatedAtActionResult;
            Assert.IsType<Book>(item.Value);

            var bookItem = item.Value as Book;
            Assert.Equal(completeBook.Author, bookItem.Author);
            Assert.Equal(completeBook.Title, bookItem.Title);
            Assert.Equal(completeBook.Description, bookItem.Description);

            //arrange
            var incompleteBook = new Book()
            {
                Author = "Author",
                Description = "Description"
            };
            //act
            _controller.ModelState.AddModelError("Title", "Title is a required field");
            var badResponce = _controller.Post(incompleteBook);
            //assert
            Assert.IsType<BadRequestObjectResult>(badResponce);
        }

    }
}
