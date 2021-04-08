// Joe Moceri
// Tax Receipts Program

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>

#define RECSIZE 50

const int PATHSIZE = 100;
const int NAMESIZE = 50;
const int SIZE = 10;

// Structure for Receipt
struct receipt
{

    char name[RECSIZE];
    int month;
    int day;
    double total;
    int num;

};

//Functions
int add(int num1, int num2);
void getFileNames(char nameIn[], char nameOut[]);
void *storeInfo(struct receipt tax_read[], FILE *ifp, int *count, double *overall, int *mem_size, int *mem_amt);
void *sizeInfo(struct receipt tax[], int *count, int *mem_size);
void *sizeTemp(struct receipt temp[], int mem_size);
void sortInfo(struct receipt tax[], struct receipt temp[], int count);
void printInfo(struct receipt tax[], int count);
void Swap(struct receipt *tax, struct receipt *inst, struct receipt *temp);
int getYear(FILE *ifp, int year);
void printFile(struct receipt tax, FILE *ofp);
void printStart(int year, FILE *ofp, char catName[]);
void printEnd(int i, int year, double overall, FILE *ofp);
void getMainCat(FILE *ifp, char catName[]);
void getSubInfo(FILE *ifp, int *amt, int num[], char name[][NAMESIZE]);
void printSubStart(FILE *ofp, char name[NAMESIZE]);
void printSubEnd(FILE *ofp, int count, double overall, char name[NAMESIZE]);
void printSub(FILE *ofp, int subAmt, char subName[][NAMESIZE], int subNum[], int tally, struct receipt tax[]);

int add(int num1, int num2)
{
	return num1 + num2;
}

int main()
{

    // Variable declarations for the dynamic array
    int mem_size = SIZE;
    int mem_amt = 0;
    
    // Initializes both the original and temporary dynamic array of structs
    struct receipt* tax = (struct receipt*)malloc(sizeof(struct receipt) * mem_size);
    struct receipt* temp = (struct receipt*)malloc(sizeof(struct receipt) * mem_size);

    // Variable string declarations
    char mainCat[NAMESIZE];
    char fileIn[NAMESIZE];
    char fileOut[NAMESIZE];
    char pathIn[PATHSIZE];
    char pathOut[PATHSIZE];

    // Copies the path location to both the input and output folder into the
    // corresponding variables
    strcpy(pathIn, "../Input/");
    strcpy(pathOut, "../Output/");

    // File pointer declarations
    FILE* ifp;
    FILE* ofp;

    // Function call to get the input and output file names
    getFileNames(fileIn, fileOut);

    // Concatenates the file name to the end of it's corresponding path
    strcat(pathIn, fileIn);
    strcat(pathOut, fileOut);

    // Opens the input file requested by user for reading
    ifp = fopen(pathIn, "r");

	// Gets the year from the file
    int year = getYear(ifp, year);
    
    // Function call to get the name of the main category
    getMainCat(ifp, mainCat);

    // Variable declarations for the subcategories
    int subAmt;
    int subNum[SIZE];
    char subName[SIZE][NAMESIZE];

    // Function call to get the amount of subcategories and each name and number
    // associated with each one
    getSubInfo(ifp, &subAmt, subNum, subName);

    int rec_amount=0; double overall=0;

    // Reads in information from file into array of structures
    tax = storeInfo(tax, ifp, &rec_amount, &overall, &mem_size, &mem_amt);

    // Function call that reduces the size of the main array based on how many
    // receipts have been read in
    tax = sizeInfo(tax, &rec_amount, &mem_size);

    // Function call that reduces the size of the temporary array based on how
    // many receipts have been read in to match the main array
    temp = sizeTemp(temp, mem_size);

    // Closes the input file pointer
    fclose(ifp);

    // Sorts the main dynamic array first by month, then day, then name,
    // then total
    sortInfo(tax, temp, rec_amount);

    // Creates the output file and sets it to be written to
    ofp=fopen(pathOut, "w");

    // Prints to the file the beginning information
    printStart(year, ofp, mainCat);

    // Prints to the file the sorted array of structures
    // seperated into sub-categories.
    printSub(ofp, subAmt, subName, subNum, rec_amount, tax);

    // Prints to the screen the sorted array of structures
    printInfo(tax, rec_amount);

    // Prints to the file the end information
    printEnd(rec_amount, year, overall, ofp);

    // Closes the output file pointer
    fclose(ofp);

    // DEBUG
    //printf("\nmem_size = %d\nmem_amt = %d\nrec_amount = %d\n", mem_size, mem_amt, rec_amount);

    // Frees up the memory of both dynamic arrays
    free(tax);
    free(temp);

    system("PAUSE");

    return 0;
}

void printInfo(struct receipt tax[], int count)
{
    // Prints out each receipt for each tax category to the terminal
    int i;
    for(i=0;i<count;i++)
	{
        printf("Name: %s\nDate: %d/%d\n                -Receipt Total: %.2lf\n", tax[i].name, tax[i].month, tax[i].day, tax[i].total);
    }

}

void getFileNames(char nameIn[], char nameOut[])
{

    // String variable declarations
    char tempIn[NAMESIZE];
    char tempOut[NAMESIZE];
    char pathIn[PATHSIZE];
    char pathOut[PATHSIZE];

    int ans;

    // While loop that prompts the user for the input file, then checks to see
    // if it exists. If so, then break. Otherwise, if the user forgot to put
    // .txt, or if the file doesn't exist, then keep asking for the input file
    while(1)
	{
        printf("Please enter the name of the input file (in file.txt format):\n");
        scanf("%s", &tempIn);

		strcpy(pathIn, "../Input/");
        strcat(pathIn, tempIn);

		if(access(pathIn, F_OK) == -1)
		{
            printf("Sorry, that file doesn't exist.\n\n");
        }
        else if(strstr(tempIn, ".txt") == NULL)
		{
            printf("Sorry, you must put .txt at the end of the filename.\n\n");
        }
		else
		{
			break;
		}

    }

    // While loop that prompts the user for the output file, then does three
    // checks. First, to make sure that the input and output file aren't the
    // same. Second, that .txt is at the end of the file name. Third, to see
    // if the file already exists, then ask if the user wants to overwrite. If
    // so, then break, otherwise prompt the user again.
    while(1)
	{
        printf("Please enter a file name to save the information to (in save.txt format):\n");
        scanf("%s", &tempOut);

		strcpy(pathOut, "../Output/");
        strcat(pathOut, tempOut);

		if(strcmp(tempIn, tempOut)==0)
		{
            printf("Sorry, you can't use the same file name to both read from and write to.\n\n");
        }
        else if(strstr(tempOut, ".txt") == NULL)
		{
            printf("Sorry, you must put .txt at the end of the filename.\n\n");
        }
        else if(access(pathOut, F_OK) == 0)
		{
            printf("This file already exists, do you want to overwrite it?(yes=1 no=0)\n");
            scanf("%d", &ans);
			if (ans == 1)
			{
				break;
			}
        }
		else
		{
			break;
		}
    }

    // Copies the final input and output file names into the main input / output
    // string variables
    strcpy(nameIn, tempIn);
    strcpy(nameOut, tempOut);

}

void printSub(FILE *ofp, int subAmt, char subName[][NAMESIZE], int subNum[], int count, struct receipt tax[])
{
    // Variable declarations
    int i, j, tally;
    double amount;

    // For loop that iterates through each subcategory. First, it prints out
    // the beginning of each subcategory. Then, the inner for loop checks the
    // sorted tax receipt list for each receipt associated with the subcategory
    // by number. When it finds it, adds one to the counter (tally) and calls
    // the function printFile that takes in the current tax receipt information
    // and prints it to the output file. At the end of each iteration of the
    // main for loop, print the ending for the subcategory.
    for(i=0;i<subAmt;i++){
        printSubStart(ofp, subName[i]);
        tally = 0;
        amount = 0;
        for(j=0;j<count;j++){
            if(tax[j].num==subNum[i]){
                tally++;
                amount+=tax[j].total;
                printFile(tax[j], ofp);
            }
        }
        printSubEnd(ofp, tally, amount, subName[i]);
    }

}

void printSubStart(FILE *ofp, char name[NAMESIZE]){

    // Prints to the output file the beginning of each subcategory
    fprintf(ofp, "==================================================================\n");
    fprintf(ofp, "%s\n", name);
    fprintf(ofp, "==================================================================\n");

}

void printSubEnd(FILE *ofp, int count, double overall, char name[NAMESIZE]){

    // Prints to the output file the end of each subcategory
    fprintf(ofp, "==================================================================\n");
    fprintf(ofp, "Total for %d receipts in %s Sub-Category is $%.2lf.\n", count, name, overall);
    fprintf(ofp, "==================================================================\n");
    fprintf(ofp, "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");

}

void getMainCat(FILE *ifp, char catName[]){

    // Temporary string variable declaration
    char tempName[NAMESIZE];

    // Scans in the name of the main category
    fscanf(ifp, "%s", &tempName);

    // Then copies that name into the main string variable passed into the
    // function
    strcpy(catName, tempName);

}

void getSubInfo(FILE *ifp, int *amt, int num[], char name[][NAMESIZE]){

    // Scans in the amount of the subcategories as declared by the user
    fscanf(ifp, "%d", &*amt);

    // For loop to read in each subcategory number and name associated with it
    int i;
    for(i=0;i<*amt;i++){
        fscanf(ifp, "%d %s", &num[i], &name[i]);
    }

}


void sortInfo(struct receipt tax[], struct receipt temp[], int count){

    // Nested for loops using a bubblesort to sort the list of tax receipts.
    // Sorts first by month, then day, then name, then total price. I.E. If
    // the month, day, and name are the same for the tax receipt, then it would
    // organize them by their prices from lowest to highest.
    // It checks each receipt like a bubblesort does, and if it's out of order,
    // then swap the contents in the dynamic array of structs. Once done, it
    // just keeps iterating until the entire list is completely sorted.
    int i, j;
    for(i=0;i<count;i++){
        for(j=0;j<count-1;j++){
            if(tax[j].month>tax[j+1].month){
                Swap(&tax[j], &tax[j+1], &temp[j]);
            }
            else if(tax[j].month==tax[j+1].month){
                if(tax[j].day>tax[j+1].day){
                    Swap(&tax[j], &tax[j+1], &temp[j]);
                }
                else if(tax[j].day==tax[j+1].day){
                    if(strcmp(tax[j].name, tax[j+1].name)>0){
                        Swap(&tax[j], &tax[j+1], &temp[j]);
                    }
                    else if(strcmp(tax[j].name, tax[j+1].name)==0){
                        if(tax[j].total>tax[j+1].total){
                            Swap(&tax[j], &tax[j+1], &temp[j]);
                        }
                    }
                }
            }
        }
    }

}

void printStart(int year, FILE *ofp, char catName[]){

    // Prints the beginning of the output file
    fprintf(ofp, "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");
    fprintf(ofp, "\nFor %s in %d\n\n", catName, year);
    fprintf(ofp, "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");

}

void printFile(struct receipt tax, FILE *ofp){

    // Prints to the output file the tax receipt taken in
    fprintf(ofp, "Name: %s\nDate: %d/%d\n                -Receipt Total: %.2lf\n", tax.name, tax.month, tax.day, tax.total);

}

void printEnd(int i, int year, double overall, FILE *ofp){

    // Prints the end of the output file with the overall information
    fprintf(ofp, "\nOverall for %d receipts in %d, total is: $%.2lf.\n\n", i, year, overall);
    fprintf(ofp, "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");

    // Prints to the terminal the same information
    printf("\nOverall for %d receipts in %d, total is: $%.2lf.\n", i, year, overall);

}

int getYear(FILE *ifp, int year){

    // Scans in the year for the tax receipts
    fscanf(ifp, "%d", &year);

    return year;
}

void Swap(struct receipt *tax, struct receipt *inst, struct receipt *temp){

    // This function is called inside the main bubblesort function. When called,
    // it will check to make sure each month, day, name and total and num are
    // sorted. If they don't equal, then swap them. Otherwise, it's unnecessary
    // to do so. I.E. If the day's are the same for each tax receipt, there's no
    // point in swapping them even if say the month is out of order, because the
    // end result is the same.
    if(tax->month!=inst->month){
        temp->month = inst->month;
        inst->month = tax->month;
        tax->month = temp->month;
    }
    if(tax->day!=inst->day){
        temp->day = inst->day;
        inst->day = tax->day;
        tax->day = temp->day;
    }
    if(strcmp(tax->name, inst->name)!=0){
        strcpy(temp->name, inst->name);
        strcpy(inst->name, tax->name);
        strcpy(tax->name, temp->name);
    }
    if(tax->total!=inst->total){
        temp->total = inst->total;
        inst->total = tax->total;
        tax->total = temp->total;
    }
    if(tax->num!=inst->num){
        temp->num = inst->num;
        inst->num = tax->num;
        tax->num = temp->num;
    }

}

void *storeInfo(struct receipt tax_read[], FILE *ifp, int *count, double *overall, int *mem_size, int *mem_amt){

    // While loop that scans in each tax receipt from the input file into the
    // dynamic array of structs and runs two checks. First, looks to see if what
    // was read in is the last receipt (-1 -1 -1 -1 -1), if so then break out.
    // Second, checks if the amount of tax receipts is close to the maximum of
    // the dynamic array of structs. If so, then double up the size. For
    // example, if the size is 40, and we read in the 38th receipt, then it
    // doubles the size of the dynamic array to 80 and keeps reading in
    // receipts.
    while(1){
        fscanf(ifp, "%s%d%d%lf%d", &tax_read[*count].name, &tax_read[*count].month, &tax_read[*count].day, &tax_read[*count].total, &tax_read[*count].num);
        if(tax_read[*count].num==-1){
            break;
        }
        *overall+=tax_read[*count].total;
        *count+=1;
        if(*count%(*mem_size-2) == 0){
            *mem_size *= 2;
            *mem_amt += 1;
            tax_read = (struct receipt*)realloc(tax_read, sizeof(struct receipt) * (*mem_size));
        }
    }

    return tax_read;

}

void *sizeInfo(struct receipt tax[], int *count, int *mem_size){

    // Once all the tax receipts are read in, this function is called to reduce
    // the size of the array by 25% i.e. 75% of it's former size if allowable.
    // First, checks to make sure the amount of receipts is above 10, then
    // checks to make sure the count is below the potential resize, and if so
    // then go ahead and reduce. At the end, return the new dynamic array of
    // structs.
    if(*count >= 10){
        if(*count < ((*mem_size*.75)-1)){
            *mem_size *= .75;
            tax = (struct receipt*)realloc(tax, sizeof(struct receipt) * (*mem_size));
        }
    }

    return tax;

}

void *sizeTemp(struct receipt temp[], int mem_size){

    // Takes in the size of the main dynamic array of structs and resizes
    // the temporary dynamic array of structs to that size
    return ((struct receipt*)realloc(temp, sizeof(struct receipt) * mem_size));

}
