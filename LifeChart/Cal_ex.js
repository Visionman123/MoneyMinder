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

// Function to calculate money that the user can save each year with multiple stages and inflation
function calculateMoneyEarnAnnualMultipleStagesWithInflation(start, end, moneySave, splitPoints, annualIncreaseRates, inflationRates) {
  const moneyEarnAnnual = [];

  // Add the start and end to the splitPoints array for convenience
  const allSplitPoints = [start, ...splitPoints, end];
  const numStages = allSplitPoints.length - 1;

  // Initialize the 2D array
  for (let i = start; i <= end; i++) {
    moneyEarnAnnual[i] = [];
  }

  // Calculate for each stage and store in the 2D array
  for (let stage = 0; stage < numStages; stage++) {
    const currentStart = allSplitPoints[stage];
    const currentEnd = allSplitPoints[stage + 1];
    const currentAnnualIncrease = annualIncreaseRates[stage];
    const currentInflation = inflationRates[stage];

    for (let i = currentStart; i <= currentEnd; i++) {
      // Apply inflation only to the pre-split period
      const inflationFactor = (i <= splitPoints[stage]) ? Math.pow(1 + currentInflation, i - start) : 1;
      moneyEarnAnnual[i][stage + 1] = (moneySave * 12) * Math.pow(1 + currentAnnualIncrease, i - currentStart) * inflationFactor;
    }
  }

  return moneyEarnAnnual;
}

// Calculate money that the user can save each year then put it into the bank with inflation
function calculateMoneyEarnAnnualWithBankROIAndInflation(start, end, moneyEarnAnnual, bank_ROI, inflationRates) {
  const moneyEarnAnnualWithBankROI = [];
  for (let i = start; i <= end; i++) {
    moneyEarnAnnualWithBankROI[i] = moneyEarnAnnual[i].reduce((acc, val, stage) => acc + val * Math.pow(1 + bank_ROI, end - i) * Math.pow(1 + inflationRates[stage], end - i), 0);
  }
  return moneyEarnAnnualWithBankROI;
}

// Calculate Total_moneyEarnAnnualWithBankROI using the provided functions with inflation
function calculateTotalMoneyEarnAnnualWithBankROIAndInflation(money_save, splitPoints, annualIncreaseRates, bank_ROI, inflationRates) {
  const moneyEarnAnnual = calculateMoneyEarnAnnualMultipleStagesWithInflation(start_age, end_age, money_save, splitPoints, annualIncreaseRates, inflationRates);
  const moneyEarnAnnualWithBankROI = calculateMoneyEarnAnnualWithBankROIAndInflation(start_age, end_age, moneyEarnAnnual, bank_ROI, inflationRates);
  return sigma(start_age, end_age, i => moneyEarnAnnualWithBankROI[i]);
}

// Example usage: Find the money_save needed to achieve the target Total_money_need
const initialGuess = 1000; // Replace with an initial guess close to the solution
const tolerance = 0.01; // Tolerance for stopping the iteration

// Specify split points, corresponding annual increase rates, and inflation rates
const splitPointsExample = [40]; // Replace with the desired split points
const annualIncreaseRatesExample = [0.1, 0.08]; // Replace with the desired annual increase rates
const inflationRatesExample = [0.06, 0.03]; // Replace with the desired inflation rates

const moneySaveNeeded = findReverseSigma(Total_money_need, x => calculateTotalMoneyEarnAnnualWithBankROIAndInflation(x, splitPointsExample, annualIncreaseRatesExample, bank_ROI, inflationRatesExample), initialGuess, tolerance);

console.log("Money_save needed:", Math.floor(moneySaveNeeded));
