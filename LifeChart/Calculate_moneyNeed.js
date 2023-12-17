// What-if input
const currentAge = 18;
const ffpAge = 65; // Financial freedom point age
const moneyFFP = 30000; // Monthly money required after financial freedom point
const inflation = 0.06; // Inflation of the country

// Capital Asset
const bankAsset = null; // Total money in the bank NOW
const bankROI = 0.06; // Bank return_of_investment (e.g., 6%)

// Event
const eventAge = 18;
const eventIncome = 0;

// Pension expected
const pension = 0;

function generateStages(stageInfo) {
    const stages = [];
  
    for (let i = 0; i < stageInfo.length; i++) {
      const { start, end, toSavePerMonth, annualIncrease } = stageInfo[i];
  
      const stage = {
        stageNumber: i+1,
        moneyEarnAnnual: [],
        moneyEarnAnnualWithBankROI: [],
        totalSave: null,
        toSavePerMonth,
        start,
        end,
        annualIncrease,
      };
  
      stages.push(stage);
    }
  
    return stages;
  }
  
// Generate stages
const stageInfo = [
  { start: 18, end: 40, toSavePerMonth: null, annualIncrease: 0.1 },
  { start: 41, end: 50, toSavePerMonth: null, annualIncrease: 0.08 },
  { start: 51, end: 65, toSavePerMonth: 5754, annualIncrease: 0.12 },
  ];
  
  const stages = generateStages(stageInfo);
  console.log(stages);
  
// Calculation Total Money needed
const moneyNeed = (moneyFFP - pension) * 12 * 25 * Math.pow(1 + inflation, ffpAge - currentAge); // Money required after FFP
const moneyEvent = eventIncome * Math.pow(1 + bankROI, ffpAge - eventAge);
const totalMoneyNeed = moneyNeed - moneyEvent;

// Function to calculate sigma of a given function
function sigma(start, end, func) {
  let sum = 0;
  for (let i = start; i < end; i++) {
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
function calculateAnnualSaving(stageNumber, start, end, monthlySaving, annualIncrease) {
  if (stageNumber != 1) {
    monthlySaving = monthlySaving * Math.pow((1+inflation),(start - currentAge)); 
  }

  const annualSaving = [];
  for (let i = 0; i <= end - start; i++) {
    const amount = (monthlySaving * 12) * Math.pow(1 + annualIncrease, start - (start - i));
    const age = start + i;
    annualSaving.push({ age, amount });
  }

  return annualSaving;
}

// Calculate money that the user can save each year then put it into the bank
function calculateAnnualSavingWithBankROI(start, end, annualSaving, bankROI) {
  const annualSavingWithBankROI = [];

  console.log(start,end);
  for (let i = 0; i <= end - start; i++) {
    const age = annualSaving[i].age;
    
    console.log(age);
    console.log(annualSaving[i]);

    const amount = annualSaving[i].amount * Math.pow(1 + bankROI, ffpAge - age);
    annualSavingWithBankROI.push({ age, amount });
    console.log(annualSavingWithBankROI[i])
  }

  return annualSavingWithBankROI;
}

// Calculate Total_moneyEarnAnnualWithBankROI using the provided functions
function calculateTotalAnnualSavingWithBankROI(stages, monthlySavingRequired, bankROI) {
  const totalAnnualSaving = [];
  stages.forEach((stage) => {
    stage.moneyEarnAnnual = calculateAnnualSaving(stage.stageNumber, stage.start, stage.end, stage.toSavePerMonth ? stage.toSavePerMonth : monthlySavingRequired, stage.annualIncrease);
    stage.moneyEarnAnnualWithBankROI = calculateAnnualSavingWithBankROI(stage.start, stage.end, stage.moneyEarnAnnual, bankROI);
    stage.moneyEarnAnnualWithBankROI.forEach((x) => {
      totalAnnualSaving.push(x);
    });
  });
  return sigma(0, totalAnnualSaving.length, (i) => totalAnnualSaving[i].amount);
}

// Example usage: Find the money_save needed to achieve the target Total_money_need
const initialGuess = 1000; // Replace with an initial guess close to the solution
const tolerance = 0.01; // Tolerance for stopping the iteration

const moneyToSavePerMonth = findReverseSigma(totalMoneyNeed, (x) => calculateTotalAnnualSavingWithBankROI(stages, x, bankROI), initialGuess, tolerance);

console.log("Money needed to save:", (moneyToSavePerMonth));

function computeMoneyLeftToSave(stages) {
  let computedSum = 0;
  stages.forEach((stage) => {
    if (stage.toSavePerMonth) {
      stage.moneyEarnAnnual = calculateMoneyEarnAnnual(stage.start, stage.end, stage.toSavePerMonth, stage.annualIncrease);
      stage.totalSave = sigma(0, stage.end - stage.start, (i) => stage.moneyEarnAnnual[i].amount);
      computedSum += stage.totalSave;
    }
  });
  return computedSum;
}

//console.log(stages);



