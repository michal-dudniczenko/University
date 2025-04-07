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
        loansRepo.add(new Loan(1, 1, 2));
        currentId++;
        loansRepo.add(new Loan(2, 2, 3));
        currentId++;
        loansRepo.add(new Loan(3,1, 1));
        currentId++;
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
    public Response getReaderLoans(int readerId, IReadersService readersService, int idFrom, int idTo) {
        if (readersService.getReader(readerId) == null) {
            return new Response(HttpStatus.NOT_FOUND, "No reader with specified id in repository.");
        }

        List<Loan> readerLoans = loansRepo.stream()
                .filter(l -> l.getReaderId() == readerId && !l.isReturned())
                .toList();

        return new Response(HttpStatus.OK, readerLoans.subList(idFrom, Math.min(idTo + 1, loansRepo.size())));
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
                return false;
            }
        }
        return true;
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
