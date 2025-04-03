package pl.edu.pwr.ztw.books.services;

import org.springframework.stereotype.Service;
import pl.edu.pwr.ztw.books.models.Reader;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

@Service
public class ReadersService implements IReadersService {
    private final static List<Reader> readersRepo = new ArrayList<>();
    private static int currentId = 1;

    static {
        readersRepo.add(new Reader(1, "Michał", "Dudniczenko"));
        currentId++;
        readersRepo.add(new Reader(2, "Jan", "Kowalski"));
        currentId++;
        readersRepo.add(new Reader(3,"Ania", "Banan"));
        currentId++;
    }

    @Override
    public Collection<Reader> getReaders() {
        return readersRepo;
    }

    @Override
    public Reader getReader(int id) {
        return readersRepo.stream()
                .filter(reader -> reader.getId() == id)
                .findAny()
                .orElse(null);
    }

    @Override
    public void addReader(Reader newReader) {
        newReader.setId(currentId);
        currentId++;
        readersRepo.add(newReader);
    }

    @Override
    public boolean updateReader(Reader reader) {
        for (int i = 0; i < readersRepo.size(); i++) {
            if (readersRepo.get(i).getId() == reader.getId()) {
                readersRepo.set(i, reader);
                return true;
            }
        }
        return false;
    }

    @Override
    public boolean deleteReader(int id, ILoansService loansService) {
        if (loansService.getBorrowedBooksCount(id) > 0) {
            return false;
        }

        for (Reader reader : readersRepo) {
            if (reader.getId() == id) {
                reader.setActive(false);
                return true;
            }
        }
        return false;
    }
}
