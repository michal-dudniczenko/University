package pl.edu.pwr.ztw.books.controllers;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import pl.edu.pwr.ztw.books.models.Book;
import pl.edu.pwr.ztw.books.services.IAuthorsService;
import pl.edu.pwr.ztw.books.services.IBooksService;

@RestController
public class BooksController {
    @Autowired
    IAuthorsService authorsService;

    @Autowired
    IBooksService booksService;

    @RequestMapping(value = "/get/books", method = RequestMethod.GET)
    public ResponseEntity<Object> getBooks() {
        return new ResponseEntity<>(booksService.getBooks(), HttpStatus.OK);
    }

    @RequestMapping(value = "/get/book/{id}", method = RequestMethod.GET)
    public ResponseEntity<Object> getBook(@PathVariable("id") int id) {
        return new ResponseEntity<>(booksService.getBook(id), HttpStatus.OK);
    }

    @RequestMapping(value = "/add/book", method = RequestMethod.POST)
    public ResponseEntity<Object> addBook(@RequestBody Book book) {
        boolean result = booksService.addBook(book, authorsService);

        return new ResponseEntity<>(result ? HttpStatus.CREATED : HttpStatus.NOT_FOUND);
    }

    @RequestMapping(value = "/update/book", method = RequestMethod.PUT)
    public ResponseEntity<Object> updateBook(@RequestBody Book book) {
        boolean result = booksService.updateBook(book, authorsService);

        return new ResponseEntity<>(result ? HttpStatus.OK : HttpStatus.NOT_FOUND);
    }

    @RequestMapping(value = "/delete/book/{id}", method = RequestMethod.DELETE)
    public ResponseEntity<Object> deleteBook(@PathVariable("id") int id) {
        boolean result = booksService.deleteBook(id);

        return new ResponseEntity<>(result ? HttpStatus.OK : HttpStatus.NOT_FOUND);
    }

    @RequestMapping(value = "/get/books/fromAuthor/{authorId}", method = RequestMethod.GET)
    public ResponseEntity<Object> getBooksFromAuthor(@PathVariable("authorId") int authorId) {
        return new ResponseEntity<>(booksService.getBooksFromAuthor(authorId), HttpStatus.OK);
    }

}
