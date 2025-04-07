package ztw.library_rest_api.controllers;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class AppController {

    @RequestMapping(value = "/", method = RequestMethod.GET)
    public String index() {
        return "hello, world!";
    }
}
