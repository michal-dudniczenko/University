package ztw.library_rest_api.controllers;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import ztw.library_rest_api.models.Reader;
import ztw.library_rest_api.models.Response;
import ztw.library_rest_api.services.IReadersService;
import ztw.library_rest_api.services.ILoansService;

@RestController
public class ReadersController {
    @Autowired
    IReadersService readersService;

    @Autowired
    ILoansService loansService;

    @RequestMapping(value = "/get/readers/{idFrom}/{idTo}", method = RequestMethod.GET)
    public ResponseEntity<Object> getReaders(@PathVariable("idFrom") int idFrom, @PathVariable("idTo") int idTo) {
        Response response = readersService.getReaders(idFrom, idTo);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/get/reader/{id}", method = RequestMethod.GET)
    public ResponseEntity<Object> getReader(@PathVariable("id") int id) {
        Response response = readersService.getReader(id);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/add/reader", method = RequestMethod.POST)
    public ResponseEntity<Object> addReader(@RequestBody Reader reader) {
        Response response = readersService.addReader(reader);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/update/reader", method = RequestMethod.PUT)
    public ResponseEntity<Object> updateReader(@RequestBody Reader reader) {
        Response response = readersService.updateReader(reader);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }

    @RequestMapping(value = "/delete/reader/{id}", method = RequestMethod.DELETE)
    public ResponseEntity<Object> deleteReader(@PathVariable("id") int id) {
        Response response = readersService.deleteReader(id, loansService);
        return new ResponseEntity<>(response.getPayload(), response.getStatus());
    }
}
