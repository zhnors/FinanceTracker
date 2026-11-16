const amounts = [20000,-350,1000];

let income = 0;
let expenses = 0;
let balance = 0;

for(const amount of amounts){
    if(amount > 0){
        income += amount;
    } 
    else {
        expenses += amount;
    }

    balance += amount;

    const sign = amount > 0 ? "+" : "";
    
    console.log(`${sign}${amount}`);
}

console.log(`Income: ${income}, Expenses: ${expenses}, Balance: ${balance}`);