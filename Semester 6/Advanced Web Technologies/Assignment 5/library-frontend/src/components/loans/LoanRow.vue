<script>
export default {
    name: "LoanRow",
    props: {
        loan: Object,
        returnOnClick: Function,
    },
}
</script>

<template>
    <tr :class="{ 'table-success': loan.returned, 'table-danger' : !loan.returned }">
        <td>{{ loan.id }}</td>
        <td>{{ loan.readerId }}</td>
        <td>{{ loan.bookId }}</td>
        <td >
            <div class="d-flex justify-content-center">
                <button v-if="!loan.returned" class="btn btn-danger w-75" :disabled="loan.returned" @click="returnOnClick(loan.readerId, loan.bookId)">Return book</button>
            </div>
        </td>
    </tr>
</template>