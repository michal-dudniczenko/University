package pl.edu.pwr.ztw.books.controllers;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import pl.edu.pwr.ztw.books.services.IBooksService;
import pl.edu.pwr.ztw.books.services.ILoansService;
import pl.edu.pwr.ztw.books.services.IReadersService;

@RestController
public class LoansController {
    @Autowired
    ILoansService loansService;

    @Autowired
    IBooksService booksService;

    @Autowired
    IReadersService readersService;

    @RequestMapping(value = "/get/loans", method = RequestMethod.GET)
    public ResponseEntity<Object> getLoans() {
        return new ResponseEntity<>(loansService.getLoans(), HttpStatus.OK);
    }

    @RequestMapping(value = "/get/loan/{id}", method = RequestMethod.GET)
    public ResponseEntity<Object> getLoan(@PathVariable("id") int id) {
        return new ResponseEntity<>(loansService.getLoan(id), HttpStatus.OK);
    }

    @RequestMapping(value = "/borrow/{readerId}/{bookId}", method = RequestMethod.POST)
    public ResponseEntity<Object> borrowBook(@PathVariable("readerId") int readerId, @PathVariable("bookId") int bookId) {
        boolean result = loansService.borrowBook(readerId, bookId, booksService, readersService);

        return new ResponseEntity<>(result ? HttpStatus.OK : HttpStatus.NOT_FOUND);
    }

    @RequestMapping(value = "/return/{readerId}/{bookId}", method = RequestMethod.POST)
    public ResponseEntity<Object> returnBook(@PathVariable("readerId") int readerId, @PathVariable("bookId") int bookId) {
        boolean result = loansService.returnBook(readerId, bookId);

        return new ResponseEntity<>(result ? HttpStatus.OK : HttpStatus.NOT_FOUND);
    }

    @RequestMapping(value = "/get/borrowedCount/{readerId}", method = RequestMethod.GET)
    public ResponseEntity<Object> getBorrowedBooksCount(@PathVariable("readerId") int readerId) {
        return new ResponseEntity<>(loansService.getBorrowedBooksCount(readerId), HttpStatus.OK);
    }

    @RequestMapping(value = "/get/borrowedBooks/{readerId}", method = RequestMethod.GET)
    public ResponseEntity<Object> getBorrowedBooks(@PathVariable("readerId") int readerId) {
        return new ResponseEntity<>(loansService.getBorrowedBooks(readerId, booksService), HttpStatus.OK);
    }

}
