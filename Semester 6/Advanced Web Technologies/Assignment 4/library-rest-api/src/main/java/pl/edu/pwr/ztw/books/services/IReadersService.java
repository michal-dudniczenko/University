package pl.edu.pwr.ztw.books.services;

import pl.edu.pwr.ztw.books.models.Reader;
import java.util.Collection;

public interface IReadersService {

    Collection<Reader> getReaders();

    Reader getReader(int id);

    void addReader(Reader newReader);

    boolean updateReader(Reader reader);

    boolean deleteReader(int id, ILoansService loansService);
}
