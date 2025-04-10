package ztw.library_rest_api.controllers;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import ztw.library_rest_api.models.Response;
import ztw.library_rest_api.services.IBooksService;
import ztw.library_rest_api.services.ILoansService;
import ztw.library_rest_api.services.IReadersService;

@CrossOrigin()
@RestController
public class LoansController {
    @Autowired
    ILoansService loansService;

    @Autowired
    IBooksService booksService;

    @Autowired
    IReadersService readersService;

    @RequestMapping(value = "/get/loans/{idFrom}/{idTo}", method = RequestMethod.GET)
    public ResponseEntity<Object> getLoans(@PathVariable("idFrom") int idFrom, @PathVariable("idTo") int idTo) {
        Response response = loansService.getLoans(idFrom, idTo);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/get/loan/{id}", method = RequestMethod.GET)
    public ResponseEntity<Object> getLoan(@PathVariable("id") int id) {
        Response response = loansService.getLoan(id);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/get/loans-count", method = RequestMethod.GET)
    public ResponseEntity<Object> getLoansCount() {
        Response response = loansService.getLoansCount();
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/get/reader-active-loans/{readerId}", method = RequestMethod.GET)
    public ResponseEntity<Object> getReaderLoans(
            @PathVariable("readerId") int readerId
    ) {
        Response response = loansService.getReaderActiveLoans(readerId, readersService);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/borrow/{readerId}/{bookId}", method = RequestMethod.POST)
    public ResponseEntity<Object> borrowBook(@PathVariable("readerId") int readerId, @PathVariable("bookId") int bookId) {
        Response response = loansService.borrowBook(readerId, bookId, booksService, readersService);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/return/{readerId}/{bookId}", method = RequestMethod.POST)
    public ResponseEntity<Object> returnBook(@PathVariable("readerId") int readerId, @PathVariable("bookId") int bookId) {
        Response response = loansService.returnBook(readerId, bookId);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }
}
