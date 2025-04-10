<script>
import LoanRow from './LoanRow.vue'

export default {
    name: "ManageLoans",
    components: {
        LoanRow,
    },
    data() {
        return {
            currentPage: 0,
            itemsPerPage: 5,

            totalItemsCount: 0,
            itemsList: [],
            noMoreItems: false,

            isSuccess: false,
            isError: false,
            successMsg: "",
            errorMsg: "",

            filterItems: "",
            filterResult: null,

            filterItemsReaderId: "",
            filterReaderIdResults: [],

            newLoanReaderId: "",
            newLoanBookId: "",
        }
    },
    methods: {
        async fetchItemsCount() {
            const url = "http://localhost:8080/get/loans-count"

            const response = await fetch(url)

            if (!response.ok) {
                return
            }

            const data = await response.text()
            this.totalItemsCount = Number(data)
        },
        async fetchItems() {
            const url = `http://localhost:8080/get/loans/${this.currentPage * this.itemsPerPage}/${this.currentPage * this.itemsPerPage + this.itemsPerPage - 1}`

            const response = await fetch(url)

            if (!response.ok) {
                return
            }

            const data = await response.json()
            this.itemsList = data

            if ((this.currentPage + 1) * this.itemsPerPage >= this.totalItemsCount) {
                this.noMoreItems = true;
            } else {
                this.noMoreItems = false;
            }
        },
        async borrowBook() {
            const url = `http://localhost:8080/borrow/${this.newLoanReaderId}/${this.newLoanBookId}`;

            const response = await fetch(url, {
                method: "POST",
            });

            if (!response.ok) {
                const errorMsg = await response.text()
                this.errorMsg = errorMsg
                this.isSuccess = false
                this.isError = true
                this.successMsg = ""
            } else {
                this.successMsg = "Book borrowed successfully."
                this.isError = false
                this.isSuccess = true
                this.errorMsg = ""
                this.currentPage = 0

                this.newLoanReaderId = ""
                this.newLoanBookId = ""

                await this.fetchItemsCount()
                await this.fetchItems()
            }
            window.scrollTo({ top: 0, behavior: 'smooth' });
            this.filterItems = ""
            this.filterResult = null
        },
        async returnBook(readerId, bookId) {
            const url = `http://localhost:8080/return/${readerId}/${bookId}`;

            const response = await fetch(url, {
                method: "POST",
            });

            if (!response.ok) {
                const errorMsg = await response.text()
                this.errorMsg = errorMsg
                this.isSuccess = false
                this.isError = true
                this.successMsg = ""
            } else {
                this.successMsg = "Book returned successfully."
                this.isError = false
                this.isSuccess = true
                this.errorMsg = ""

                await this.fetchItems()
            }
            window.scrollTo({ top: 0, behavior: 'smooth' });
        },
        hideAlerts() {
            this.isSuccess = false
            this.isError = false
            this.successMsg = ""
            this.errorMsg = ""
        },
        async nextPage() {
            this.hideAlerts()
            this.currentPage++
            await this.fetchItems()
        },
        async previousPage() {
            this.hideAlerts()
            this.currentPage--
            await this.fetchItems()
        },
        async onFilterInput(input) {
            this.filterItemsReaderId = ""
            this.filterReaderIdResults = []

            this.filterItems = input.target.value

            if (this.filterItems.length === 0) {
                this.filterResult = null;
                return;
            }

            const url = `http://localhost:8080/get/loan/${this.filterItems}`

            const response = await fetch(url)

            if (!response.ok) {
                this.filterResult = null;
                return;
            }

            const data = await response.json()
            this.filterResult = data
        },
        async onFilterReaderIdInput(input) {
            this.filterItems = "";
            this.filterResult = null

            this.filterItemsReaderId = input.target.value

            if (this.filterItemsReaderId.length === 0) {
                this.filterReaderIdResults = [];
                return;
            }

            const url = `http://localhost:8080/get/reader-active-loans/${this.filterItemsReaderId}`

            const response = await fetch(url)

            if (!response.ok) {
                this.filterReaderIdResults = [];
                return;
            }

            const data = await response.json()
            this.filterReaderIdResults = data
        },
    },
    mounted() {
        this.fetchItemsCount()
        this.fetchItems()
    }
}
</script>

<template>
    <div class="container-fluid">
        <div class="row justify-content-center">
            <div class="col-lg-8 col-12">
                <!-- success alert -->
                <div id="alert-success" class="alert alert-success alert-dismissible fade show" role="alert"
                    v-if="isSuccess">
                    {{ successMsg }}
                    <button type="button" class="btn-close" @click="hideAlerts"></button>
                </div>

                <!-- error alert -->
                <div id="alert-error" class="alert alert-danger alert-dismissible fade show" role="alert"
                    v-if="isError">
                    Error: {{ errorMsg }}
                    <button type="button" class="btn-close" @click="hideAlerts"></button>
                </div>

                <!-- items browse table -->
                <div class="mb-5 bg-dark text-white p-4 rounded">
                    <div class="d-flex mb-3 justify-content-between">
                        <h2 class="me-5 text-nowrap">All loans</h2>
                        <div class="d-flex flex-wrap justify-content-end gap-2">
                            <input class="rounded p-1" :value="filterItemsReaderId" @input="onFilterReaderIdInput" type="number"
                                placeholder="Filter by reader id" >

                            <input class="rounded p-1" :value="filterItems" @input="onFilterInput" type="number"
                                placeholder="Filter by loan id">
                        </div>
                        
                    </div>

                    <table class="table table-dark table-hover table-bordered text-white rounded overflow-hidden">
                        <thead>
                            <tr>
                                <th style="width: 10%;">ID</th>
                                <th style="width: 20%;">Reader ID</th>
                                <th style="width: 20%;">Book ID</th>
                                <th style="width: 50%;"></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-if="itemsList.length === 0">
                                <td colspan="4" class="text-center p-3">No loans in database.</td>
                            </tr>
                            <tr v-else-if="filterItems.length != 0 && filterResult === null">
                                <td colspan="4" class="text-center p-3">
                                    No loan with specified id.
                                </td>
                            </tr>
                            <LoanRow 
                                v-else-if="filterItems.length != 0" 
                                :loan="filterResult"
                                :returnOnClick="returnBook"
                            />
                            <tr v-else-if="filterItemsReaderId.length != 0 && filterReaderIdResults.length === 0">
                                <td colspan="4" class="text-center p-3">
                                    Reader with specified id does not have any active loans.
                                </td>
                            </tr>
                            <LoanRow
                                v-else-if="filterItemsReaderId.length != 0"
                                v-for="readerLoan in filterReaderIdResults" 
                                :key="readerLoan.id" 
                                :loan="readerLoan"
                                :returnOnClick="returnBook"
                            />
                            <LoanRow 
                                v-else 
                                v-for="loan in itemsList" 
                                :key="loan.id" 
                                :loan="loan"
                                :returnOnClick="returnBook"
                            />
                        </tbody>
                    </table>
                    <div class="d-flex justify-content-between mt-4">
                        <button @click="previousPage" class="btn btn-secondary w-25"
                            :disabled="currentPage === 0">Previous</button>
                        <button @click="nextPage" class="btn btn-secondary w-25" :disabled="noMoreItems">Next</button>
                    </div>

                </div>

                <!-- borrow book form -->
                <form class="bg-dark rounded  text-white p-4" @submit.prevent="borrowBook" autocomplete="off">
                    <h2 class="mb-3">Borrow book</h2>
                    <div class="mb-3">
                        <label for="borrowBookReaderId" class="form-label">Reader ID</label>
                        <input type="number" v-model="newLoanReaderId" class="form-control" id="borrowBookReaderId"
                            required>
                    </div>
                    <div class="mb-3">
                        <label for="borrowBookBookId" class="form-label">Book ID</label>
                        <input type="number" v-model="newLoanBookId" class="form-control" id="borrowBookBookId"
                            required>
                    </div>
                    <button type="submit" class="btn btn-success px-5">Submit</button>
                </form>

            </div>
        </div>
    </div>

</template>