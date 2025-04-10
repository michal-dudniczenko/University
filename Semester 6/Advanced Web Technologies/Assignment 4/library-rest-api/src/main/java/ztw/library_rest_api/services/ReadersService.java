package ztw.library_rest_api.services;

import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import ztw.library_rest_api.models.Reader;
import ztw.library_rest_api.models.Response;

import java.util.ArrayList;
import java.util.List;

@Service
public class ReadersService implements IReadersService {
    private final static List<Reader> readersRepo = new ArrayList<>();
    private static int currentId = 1;

    static {
        readersRepo.add(new Reader(currentId++, "Jan", "Kowalski"));
        readersRepo.add(new Reader(currentId++, "Piotr", "Nowak"));
        readersRepo.add(new Reader(currentId++, "Anna", "Wiśniewska"));
        readersRepo.add(new Reader(currentId++, "Michał", "Dąbrowski"));
        readersRepo.add(new Reader(currentId++, "Katarzyna", "Lewandowska"));
        readersRepo.add(new Reader(currentId++, "Tomasz", "Wójcik"));
        readersRepo.add(new Reader(currentId++, "Agnieszka", "Kamińska"));
        readersRepo.add(new Reader(currentId++, "Mateusz", "Zieliński"));
        readersRepo.add(new Reader(currentId++, "Barbara", "Szymańska"));
        readersRepo.add(new Reader(currentId++, "Andrzej", "Woźniak"));
        readersRepo.add(new Reader(currentId++, "Ewa", "Kozłowska"));
        readersRepo.add(new Reader(currentId++, "Grzegorz", "Jankowski"));
        readersRepo.add(new Reader(currentId++, "Magdalena", "Mazur"));
        readersRepo.add(new Reader(currentId++, "Krzysztof", "Kwiatkowski"));
        readersRepo.add(new Reader(currentId++, "Monika", "Krawczyk"));
        readersRepo.add(new Reader(currentId++, "Paweł", "Zalewski"));
        readersRepo.add(new Reader(currentId++, "Joanna", "Grabowska"));
        readersRepo.add(new Reader(currentId++, "Rafał", "Piotrowski"));
        readersRepo.add(new Reader(currentId++, "Natalia", "Nowacka"));
        readersRepo.add(new Reader(currentId++, "Sebastian", "Czarnecki"));
    }

    @Override
    public Response getReaders(int idFrom, int idTo) {
        if (idFrom >= readersRepo.size()) {
            return new Response(HttpStatus.BAD_REQUEST, "Starting index out of bounds.");
        }

        return new Response(HttpStatus.OK, readersRepo.subList(idFrom, Math.min(idTo + 1, readersRepo.size())));
    }

    @Override
    public Response getReader(int id) {
        for (Reader r : readersRepo) {
            if (r.getId() == id) {
                return new Response(HttpStatus.OK, r);
            }
        }
        return new Response(HttpStatus.NOT_FOUND);
    }

    @Override
    public Response getReadersCount() {
        return new Response(HttpStatus.OK, readersRepo.size());
    }

    @Override
    public Response addReader(Reader newReader) {
        newReader.setId(currentId);
        currentId++;
        readersRepo.add(newReader);
        return new Response(HttpStatus.OK, newReader);
    }

    @Override
    public Response updateReader(Reader reader) {
        for (int i = 0; i < readersRepo.size(); i++) {
            if (readersRepo.get(i).getId() == reader.getId()) {
                readersRepo.set(i, reader);
                return new Response(HttpStatus.OK);
            }
        }
        return new Response(HttpStatus.NOT_FOUND, "No reader with specified id in repository.");
    }

    @Override
    public Response deleteReader(int id, ILoansService loansService) {
        for (int i = 0; i < readersRepo.size(); i++) {
            if (readersRepo.get(i).getId() == id) {
                if (loansService.getBorrowedBooksCount(id) > 0) {
                    return new Response(HttpStatus.BAD_REQUEST, "Reader must first return all borrowed books.");
                }
                loansService.readerDeletionClear(id);
                readersRepo.remove(i);
                return new Response(HttpStatus.OK);
            }
        }
        return new Response(HttpStatus.NOT_FOUND, "No reader with specified id in repository.");
    }
}
