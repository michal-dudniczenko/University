package pl.edu.pwr.ztw.books.controllers;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import pl.edu.pwr.ztw.books.models.Reader;
import pl.edu.pwr.ztw.books.services.IReadersService;
import pl.edu.pwr.ztw.books.services.ILoansService;

@RestController
public class ReadersController {
    @Autowired
    IReadersService readersService;

    @Autowired
    ILoansService loansService;

    @RequestMapping(value = "/get/readers", method = RequestMethod.GET)
    public ResponseEntity<Object> getReaders() {
        return new ResponseEntity<>(readersService.getReaders(), HttpStatus.OK);
    }

    @RequestMapping(value = "/get/reader/{id}", method = RequestMethod.GET)
    public ResponseEntity<Object> getReader(@PathVariable("id") int id) {
        return new ResponseEntity<>(readersService.getReader(id), HttpStatus.OK);
    }

    @RequestMapping(value = "/add/reader", method = RequestMethod.POST)
    public ResponseEntity<Object> addReader(@RequestBody Reader reader) {
        readersService.addReader(reader);
        return new ResponseEntity<>(HttpStatus.CREATED);
    }

    @RequestMapping(value = "/update/reader", method = RequestMethod.PUT)
    public ResponseEntity<Object> updateReader(@RequestBody Reader reader) {
        boolean result = readersService.updateReader(reader);

        return new ResponseEntity<>(result ? HttpStatus.OK : HttpStatus.NOT_FOUND);
    }

    @RequestMapping(value = "/delete/reader/{id}", method = RequestMethod.DELETE)
    public ResponseEntity<Object> deleteReader(@PathVariable("id") int id) {
        boolean result = readersService.deleteReader(id, loansService);

        return new ResponseEntity<>(result ? HttpStatus.OK : HttpStatus.NOT_FOUND);
    }
}
