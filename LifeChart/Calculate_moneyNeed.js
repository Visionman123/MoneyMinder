// What-if input
const current_age = 18;
const ffp_age = 65; // Financial freedom point age
const money_ffp = 30000; // Monthly money required after financial freedom point
const inflation = 0.06; // Inflation of the country
// Capital Asset
const bank_asset = null; // Total money in the bank NOW
const bank_ROI = 0.06; // Bank return_of_investment (Ex: 6%)
// Stage of life
const start_age = 18;
const end_age = 65;
const Annual_increase = 0.1;
// Event
const event_age = 18;
const event_income = 0;
// Pension expected
const pension = 0; 

// Calculation Total Money needed 
const Money_need = (money_ffp - pension) * 12 * 25 * Math.pow(1 + inflation, ffp_age - current_age); // Money required after FFP
const Money_event = event_income * Math.pow(1 + bank_ROI, ffp_age - event_age);
const Total_money_need = Money_need - Money_event;

// Function to calculate sigma of a given function
function sigma(start, end, func) {
  let sum = 0;
  for (let i = start; i <= end; i++) {
    sum += func(i);
  }
  return sum;
}

// Function to find the reverse of sigma using Newton-Raphson method
function findReverseSigma(targetSum, func, initialGuess, tolerance) {
  let x = initialGuess;
  let previousX;

  do {
    previousX = x;
    x = x - (func(x) - targetSum) / calculateDerivative(func, x);
  } while (Math.abs(x - previousX) > tolerance);

  return x;
}

// Function to calculate the derivative of a given function using a small step value
function calculateDerivative(func, x) {
  const step = 0.0001; // Small step value
  return (func(x + step) - func(x)) / step;
}

// Calculate money that the user can save each year
function calculateMoneyEarnAnnual(start, end, moneySave, annualIncrease) {
  const moneyEarnAnnual = [];
  for (let i = start; i <= end; i++) {
    moneyEarnAnnual[i] = (moneySave * 12) * Math.pow(1 + annualIncrease, i - start);
  }
  return moneyEarnAnnual;
}

// Calculate money that the user can save each year then put it into the bank
function calculateMoneyEarnAnnualWithBankROI(start, end, moneyEarnAnnual, bank_ROI) {
  const moneyEarnAnnualWithBankROI = [];
  for (let i = start; i <= end; i++) {
    moneyEarnAnnualWithBankROI[i] = moneyEarnAnnual[i] * Math.pow(1 + bank_ROI, end - i);
  }
  return moneyEarnAnnualWithBankROI;
}

// Calculate Total_moneyEarnAnnualWithBankROI using the provided functions
function calculateTotalMoneyEarnAnnualWithBankROI(money_save, Annual_increase, bank_ROI) {
  const moneyEarnAnnual = calculateMoneyEarnAnnual(start_age, end_age, money_save, Annual_increase);
  const moneyEarnAnnualWithBankROI = calculateMoneyEarnAnnualWithBankROI(start_age, end_age, moneyEarnAnnual, bank_ROI);
  return sigma(start_age, end_age, i => moneyEarnAnnualWithBankROI[i]);
}

// Example usage: Find the money_save needed to achieve the target Total_money_need
const initialGuess = 1000; // Replace with an initial guess close to the solution
const tolerance = 0.01; // Tolerance for stopping the iteration

const moneySaveNeeded = findReverseSigma(Total_money_need, x => calculateTotalMoneyEarnAnnualWithBankROI(x, Annual_increase, bank_ROI), initialGuess, tolerance);

console.log("Money_save needed:", Math.floor(moneySaveNeeded));
