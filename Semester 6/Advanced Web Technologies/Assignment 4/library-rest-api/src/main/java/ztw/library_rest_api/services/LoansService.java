package ztw.library_rest_api.services;

import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import ztw.library_rest_api.models.Loan;
import ztw.library_rest_api.models.Response;

import java.util.ArrayList;
import java.util.List;

@Service
public class LoansService implements ILoansService{
    private final static List<Loan> loansRepo = new ArrayList<>();
    private static int currentId = 1;

    private final static int maxBorrowedBooks = 3;
    
    static {
        loansRepo.add(new Loan(currentId++, 8, 7));
        loansRepo.add(new Loan(currentId++, 5, 8));
        loansRepo.add(new Loan(currentId++, 1, 5));
        loansRepo.add(new Loan(currentId++, 3, 12));
        loansRepo.add(new Loan(currentId++, 10, 16));
        loansRepo.add(new Loan(currentId++, 2, 3));
        loansRepo.add(new Loan(currentId++, 6, 18));
        loansRepo.add(new Loan(currentId++, 9, 6));
        loansRepo.add(new Loan(currentId++, 4, 2));
        loansRepo.add(new Loan(currentId++, 1, 11));
        loansRepo.add(new Loan(currentId++, 7, 9));
        loansRepo.add(new Loan(currentId++, 5, 7));
        loansRepo.add(new Loan(currentId++, 6, 17));
        loansRepo.add(new Loan(currentId++, 9, 1));
        loansRepo.add(new Loan(currentId++, 8, 15));
        loansRepo.add(new Loan(currentId++, 4, 14));
        loansRepo.add(new Loan(currentId++, 10, 20));
        loansRepo.add(new Loan(currentId++, 2, 9));
        loansRepo.add(new Loan(currentId++, 1, 13));
        loansRepo.add(new Loan(currentId++, 7, 4));
        loansRepo.add(new Loan(currentId++, 6, 3));
        loansRepo.add(new Loan(currentId++, 5, 12));
        loansRepo.add(new Loan(currentId++, 3, 13));
        loansRepo.add(new Loan(currentId++, 4, 18));
        loansRepo.add(new Loan(currentId++, 10, 19));
        loansRepo.add(new Loan(currentId++, 2, 6));
        loansRepo.add(new Loan(currentId++, 3, 4));
        loansRepo.add(new Loan(currentId++, 9, 5));
        loansRepo.add(new Loan(currentId++, 8, 19));
        loansRepo.add(new Loan(currentId++, 7, 10));
    }

    @Override
    public Response getLoans(int idFrom, int idTo) {
        if (idFrom >= loansRepo.size()) {
            return new Response(HttpStatus.BAD_REQUEST, "Starting index out of bounds.");
        }

        return new Response(HttpStatus.OK, loansRepo.subList(idFrom, Math.min(idTo + 1, loansRepo.size())));
    }

    @Override
    public Response getLoan(int id) {
        for (Loan l : loansRepo) {
            if (l.getId() == id) {
                return new Response(HttpStatus.OK, l);
            }
        }
        return new Response(HttpStatus.NOT_FOUND);
    }

    @Override
    public Response getLoansCount() {
        return new Response(HttpStatus.OK, loansRepo.size());
    }

    @Override
    public Response getReaderActiveLoans(int readerId, IReadersService readersService) {
        if (readersService.getReader(readerId) == null) {
            return new Response(HttpStatus.NOT_FOUND, "No reader with specified id in repository.");
        }

        List<Loan> readerLoans = loansRepo.stream()
                .filter(l -> l.getReaderId() == readerId && !l.isReturned())
                .toList();

        return new Response(HttpStatus.OK, readerLoans);
    }

    @Override
    public Response borrowBook(int readerId, int bookId, IBooksService booksService, IReadersService readersService) {
        if (booksService.getBook(bookId) == null) {
            return new Response(HttpStatus.BAD_REQUEST, "No book with specified id in repository.");
        } else if (readersService.getReader(readerId) == null) {
            return new Response(HttpStatus.BAD_REQUEST, "No reader with specified id in repository.");
        } else if (isBookBorrowed(bookId)) {
            return new Response(HttpStatus.BAD_REQUEST, "This book is already borrowed.");
        } else if (getBorrowedBooksCount(readerId) >= maxBorrowedBooks) {
            return new Response(HttpStatus.BAD_REQUEST, "This reader is already at borrow limit.");
        }

        loansRepo.add(new Loan(currentId, readerId, bookId));
        return new Response(HttpStatus.OK);
    }

    @Override
    public Response returnBook(int readerId, int bookId) {
        for (Loan loan : loansRepo) {
            if (loan.getReaderId() == readerId && loan.getBookId() == bookId) {
                if (loan.isReturned()) {
                    return new Response(HttpStatus.BAD_REQUEST, "This book has already been returned.");
                }
                loan.setReturned(true);
                return new Response(HttpStatus.OK);
            }
        }
        return new Response(HttpStatus.NOT_FOUND, "No loan with specified reader id and book id pair in repository.");
    }

    @Override
    public int getBorrowedBooksCount(int readerId) {
        int booksCount = 0;
        for (Loan loan : loansRepo) {
            if (loan.getReaderId() == readerId && !loan.isReturned()) {
                booksCount++;
            }
        }
        return booksCount;
    }

    @Override
    public boolean isBookBorrowed(int bookId) {
        for (Loan loan : loansRepo) {
            if (loan.getBookId() == bookId && !loan.isReturned()) {
                return true;
            }
        }
        return false;
    }

    @Override
    public void bookDeletionClear(int bookId) {
        for (Loan loan : loansRepo) {
            if (loan.getBookId() == bookId) {
                loan.setBookId(-1);
            }
        }
    }

    @Override
    public void readerDeletionClear(int readerId) {
        for (Loan loan : loansRepo) {
            if (loan.getReaderId() == readerId) {
                loan.setReaderId(-1);
            }
        }
    }
}
