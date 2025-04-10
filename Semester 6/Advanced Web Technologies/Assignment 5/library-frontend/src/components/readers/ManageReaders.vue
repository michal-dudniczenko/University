<script>
import ReaderRow from './ReaderRow.vue'

export default {
    name: "ManageReaders",
    components: {
        ReaderRow,
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

            editItemId: -1,

            newReaderFirstName: "",
            newReaderLastName: "",
        }
    },
    methods: {
        async fetchItemsCount() {
            const url = "http://localhost:8080/get/readers-count"

            const response = await fetch(url)

            if (!response.ok) {
                return
            }

            const data = await response.text()
            this.totalItemsCount = Number(data)
        },
        async fetchItems() {
            const url = `http://localhost:8080/get/readers/${this.currentPage * this.itemsPerPage}/${this.currentPage * this.itemsPerPage + this.itemsPerPage - 1}`

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
        async addItem() {
            this.editItemId = -1

            const url = "http://localhost:8080/add/reader";

            const response = await fetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ firstName: this.newReaderFirstName, lastName: this.newReaderLastName })
            });

            if (!response.ok) {
                const errorMsg = await response.text()
                this.errorMsg = errorMsg
                this.isSuccess = false
                this.isError = true
                this.successMsg = ""
            } else {
                this.successMsg = "Reader added successfully."
                this.isError = false
                this.isSuccess = true
                this.errorMsg = ""
                this.currentPage = 0

                this.newReaderFirstName = ""
                this.newReaderLastName = ""

                await this.fetchItemsCount()
                await this.fetchItems()
            }
            window.scrollTo({ top: 0, behavior: 'smooth' });
            this.filterItems = ""
            this.filterResult = null
        },
        async deleteItem(itemId) {
            this.editItemId = -1

            const url = `http://localhost:8080/delete/reader/${itemId}`;

            const response = await fetch(url, {
                method: "DELETE"
            });

            if (!response.ok) {
                const errorMsg = await response.text()
                this.errorMsg = errorMsg
                this.isSuccess = false
                this.isError = true
                this.successMsg = ""
            } else {
                this.successMsg = "Reader deleted successfully."
                this.isError = false
                this.isSuccess = true
                this.errorMsg = ""
                this.currentPage = 0
                await this.fetchItemsCount()
                await this.fetchItems()
            }
            window.scrollTo({ top: 0, behavior: 'smooth' });
            this.filterItems = ""
            this.filterResult = null
        },
        async editItem(itemId, newReaderFirstName, newReaderLastName) {
            const url = "http://localhost:8080/update/reader";

            const response = await fetch(url, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ id: itemId, firstName: newReaderFirstName, lastName: newReaderLastName })
            });

            if (!response.ok) {
                const errorMsg = await response.text()
                this.errorMsg = errorMsg
                this.isSuccess = false
                this.isError = true
                this.successMsg = ""
            } else {
                this.successMsg = "Reader edited successfully."
                this.isError = false
                this.isSuccess = true
                this.errorMsg = ""
                this.currentPage = 0
                this.editItemId = -1
                await this.fetchItemsCount()
                await this.fetchItems()
            }
            window.scrollTo({ top: 0, behavior: 'smooth' });
            this.filterItems = ""
            this.filterResult = null
        },
        hideAlerts() {
            this.isSuccess = false
            this.isError = false
            this.successMsg = ""
            this.errorMsg = ""
        },
        async nextPage() {
            this.editItemId = -1
            this.hideAlerts()
            this.currentPage++
            await this.fetchItems()
        },
        async previousPage() {
            this.editItemId = -1
            this.hideAlerts()
            this.currentPage--
            await this.fetchItems()
        },
        async onFilterInput(input) {
            this.editItemId = -1
            this.filterItems = input.target.value

            if (this.filterItems.length === 0) {
                this.filterResult = null;
                return;
            }

            const url = `http://localhost:8080/get/reader/${this.filterItems}`

            const response = await fetch(url)

            if (!response.ok) {
                this.filterResult = null;
                return;
            }

            const data = await response.json()
            this.filterResult = data
        },
        setEditItem(itemId) {
            if (this.editItemId === itemId) {
                this.editItemId = -1;
            } else {
                this.editItemId = itemId;
            }
        }
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
                    <div class="d-flex justify-content-between mb-3">
                        <h2>All readers</h2>
                        <input :value="filterItems" @input="onFilterInput" type="number"
                            placeholder="Filter by reader id" class="rounded p-1">
                    </div>


                    <table
                        class="table table-dark table-striped table-hover table-bordered text-white rounded overflow-hidden">

                        <thead>
                            <tr>
                                <th style="width: 10%;">ID</th>
                                <th style="width: 27.5%;">First name</th>
                                <th style="width: 27.5%;">Last name</th>
                                <th style="width: 35%;"></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-if="itemsList.length === 0">
                                <td colspan="5" class="text-center p-3">No readers in database.</td>
                            </tr>
                            <tr v-else-if="filterItems.length != 0 && filterResult === null">
                                <td v-if="filterResult === null" colspan="5" class="text-center p-3">No reader with
                                    specified id.</td>
                            </tr>
                            <ReaderRow v-else-if="filterItems.length != 0" :reader="filterResult"
                                :editReaderId="editItemId" :editOnClick="setEditItem" :deleteOnClick="deleteItem"
                                :submitEditOnClick="editItem" />
                            <ReaderRow v-else v-for="reader in itemsList" :key="reader.id" :reader="reader"
                                :editReaderId="editItemId" :editOnClick="setEditItem" :deleteOnClick="deleteItem"
                                :submitEditOnClick="editItem" />
                        </tbody>
                    </table>
                    <div class="d-flex justify-content-between mt-4">
                        <button @click="previousPage" class="btn btn-secondary w-25"
                            :disabled="currentPage === 0">Previous</button>
                        <button @click="nextPage" class="btn btn-secondary w-25" :disabled="noMoreItems">Next</button>
                    </div>

                </div>

                <!-- add new item form -->
                <form class="bg-dark rounded  text-white p-4" @submit.prevent="addItem" autocomplete="off">
                    <h2 class="mb-3">Add new reader</h2>
                    <div class="mb-3">
                        <label for="addReaderFirstName" class="form-label">First name</label>
                        <input type="text" v-model="newReaderFirstName" class="form-control" id="addReaderFirstName"
                            required>
                    </div>
                    <div class="mb-3">
                        <label for="addReaderLastName" class="form-label">Last name</label>
                        <input type="text" v-model="newReaderLastName" class="form-control" id="addReaderLastName"
                            required>
                    </div>
                    <button type="submit" class="btn btn-success px-5">Submit</button>
                </form>

            </div>
        </div>
    </div>

</template>