package ztw.library_rest_api.controllers;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import ztw.library_rest_api.models.Author;
import ztw.library_rest_api.models.Response;
import ztw.library_rest_api.services.IAuthorsService;
import ztw.library_rest_api.services.IBooksService;

@CrossOrigin
@RestController
public class AuthorsController {
    @Autowired
    IAuthorsService authorsService;

    @Autowired
    IBooksService booksService;

    @RequestMapping(value = "/get/authors/{idFrom}/{idTo}", method = RequestMethod.GET)
    public ResponseEntity<Object> getAuthors(@PathVariable("idFrom") int idFrom, @PathVariable("idTo") int idTo) {
        Response response = authorsService.getAuthors(idFrom, idTo);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/get/author/{id}", method = RequestMethod.GET)
    public ResponseEntity<Object> getAuthor(@PathVariable("id") int id) {
        Response response = authorsService.getAuthor(id);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/get/authors-count", method = RequestMethod.GET)
    public ResponseEntity<Object> getAuthorsCount() {
        Response response = authorsService.getAuthorsCount();
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/add/author", method = RequestMethod.POST)
    public ResponseEntity<Object> addAuthor(@RequestBody Author author) {
        Response response = authorsService.addAuthor(author);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/update/author", method = RequestMethod.PUT)
    public ResponseEntity<Object> updateAuthor(@RequestBody Author author) {
        Response response = authorsService.updateAuthor(author);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/delete/author/{id}", method = RequestMethod.DELETE)
    public ResponseEntity<Object> deleteAuthor(@PathVariable("id") int id) {
        Response response = authorsService.deleteAuthor(id, booksService);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }
}
