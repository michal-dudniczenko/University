package ztw.library_rest_api.controllers;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import ztw.library_rest_api.models.Book;
import ztw.library_rest_api.models.Response;
import ztw.library_rest_api.services.IAuthorsService;
import ztw.library_rest_api.services.IBooksService;
import ztw.library_rest_api.services.ILoansService;

@RestController
public class BooksController {
    @Autowired
    IAuthorsService authorsService;

    @Autowired
    ILoansService loansService;

    @Autowired
    IBooksService booksService;

    @RequestMapping(value = "/get/books/{idFrom}/{idTo}", method = RequestMethod.GET)
    public ResponseEntity<Object> getBooks(@PathVariable("idFrom") int idFrom, @PathVariable("idTo") int idTo) {
        Response response = booksService.getBooks(idFrom, idTo);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/get/book/{id}", method = RequestMethod.GET)
    public ResponseEntity<Object> getBook(@PathVariable("id") int id) {
        Response response = booksService.getBook(id);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/add/book", method = RequestMethod.POST)
    public ResponseEntity<Object> addBook(@RequestBody Book book) {
        Response response = booksService.addBook(book, authorsService);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/update/book", method = RequestMethod.PUT)
    public ResponseEntity<Object> updateBook(@RequestBody Book book) {
        Response response = booksService.updateBook(book, authorsService);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/delete/book/{id}", method = RequestMethod.DELETE)
    public ResponseEntity<Object> deleteBook(@PathVariable("id") int id) {
        Response response = booksService.deleteBook(id, loansService);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }
}
