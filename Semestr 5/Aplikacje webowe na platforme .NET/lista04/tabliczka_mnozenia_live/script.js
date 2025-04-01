document.getElementById("numberForm").addEventListener("submit", function(event) {
    event.preventDefault();

    document.getElementById("error").style.display = "none";

    let userNumber = Number(document.getElementById("userNumber").value);

    if (isNaN(userNumber) || userNumber < 5 || userNumber > 20) {
        document.getElementById("error").style.display = "block";
        userNumber = 10;
    }

    const randomNumbers = [];

    for (let i = 0; i < userNumber; i++) {
        const randomNumber = Math.floor(Math.random() * 99 + 1);
        randomNumbers.push(randomNumber);
    }

    const table = document.getElementById("tabliczkaMnozenia");

    table.innerHTML = "";

    const numberOfRows = userNumber + 1;

    for (let i = 0; i < numberOfRows; i++) {
        const row = table.insertRow();

        for (let j = 0; j < numberOfRows; j++) {
            const cell = row.insertCell();

            if (i == 0 || j == 0) {
                cell.style.backgroundColor = "yellow";
                cell.style.fontSize = "larger";
            }

            if (i == 0 && j == 0) {
                continue;
            } else if (i == 0) {
                cell.innerHTML = randomNumbers[j - 1];
            } else if (j == 0) {
                cell.innerHTML = randomNumbers[i - 1];
            } else {
                const result = randomNumbers[i - 1] * randomNumbers[j - 1];
                const reszta = result % 3;
                switch (reszta) {
                    case 0:
                        cell.className = "reszta0";
                        break;
                    case 1:
                        cell.className = "reszta1";
                        break;
                    case 2:
                        cell.className = "reszta2";
                        break;
                }
                cell.innerHTML = result;
            }
        }
        
    }
})