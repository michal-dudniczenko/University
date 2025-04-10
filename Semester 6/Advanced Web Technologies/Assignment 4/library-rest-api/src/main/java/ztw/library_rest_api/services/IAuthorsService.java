package ztw.library_rest_api.services;

import ztw.library_rest_api.models.Author;
import ztw.library_rest_api.models.Response;

public interface IAuthorsService {

    Response getAuthors(int idFrom, int idTo);

    Response getAuthor(int id);

    Response getAuthorsCount();

    Response addAuthor(Author newAuthor);

    Response updateAuthor(Author author);

    Response deleteAuthor(int id, IBooksService booksService);


}
