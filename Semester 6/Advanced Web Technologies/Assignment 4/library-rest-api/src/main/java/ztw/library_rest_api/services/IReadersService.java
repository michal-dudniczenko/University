package ztw.library_rest_api.services;

import ztw.library_rest_api.models.Reader;
import ztw.library_rest_api.models.Response;

public interface IReadersService {

    Response getReaders(int idFrom, int idTo);

    Response getReader(int id);

    Response getReadersCount();

    Response addReader(Reader newReader);

    Response updateReader(Reader reader);

    Response deleteReader(int id, ILoansService loansService);
}
