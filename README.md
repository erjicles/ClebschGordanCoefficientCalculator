# Clebsch-Gordan Coefficient Calculator

## Overview

.NET console application written in C# to calculate
[Clebsch-Gordan coefficients](https://en.wikipedia.org/wiki/Clebsch%E2%80%93Gordan_coefficients)
using recursion equations given in [1](#ref-1).

## Usage

This is a text-based console application. When the program
starts, it asks for the angular momentum parameters of the
desired Clebsch-Gordan coefficient:

j1, j2, m, j, m1, and m2

Provided that these satisfy various constraints, the program
will calculate the coefficient using the recursion equations
and display the result.

For some examples of parameters to try, see 
[here](https://en.wikipedia.org/wiki/Table_of_Clebsch%E2%80%93Gordan_coefficients#Specific_values).

## Motivation

I was originally assigned the task of writing a computer
program to calculate the Clebsch-Gordan coefficients using 
the recursion equations in grad school. Partially due to
procrastination and partially due to not fully understanding
the equations, I failed to complete the assignment and in
desperation utilized a direct closed-form calculation of the
coefficients given [here](https://en.wikipedia.org/wiki/Table_of_Clebsch%E2%80%93Gordan_coefficients#Formulation).

While this successfully calculated the coefficients, I wasn't
satisfied with the solution because it didn't utilize the
recursion equations. Even long after I finished grad school,
every now and then I felt the urge to revisit the problem.

Finally, ten years after leaving grad school, I've returned to
the problem and succeeded in writing a program that produces
the exact coefficients utilizing the recursion equations as
the original problem required.

## Recursion Equations

The following recursion equations are used to calculate the
Clebsch-Gordan coefficients:

![Equation 1 - left hand side](https://latex.codecogs.com/gif.latex?%5Csqrt%7B%28j%20%5Cmp%20m%29%28j%20%5Cpm%20m%20&plus;%201%29%7D%3Cj_1j_2%3B%20m_1m_2%7Cj_1j_2%3B%20j%2C%20m%20%5Cpm%201%3E)

![Equation 1 - right hand side part 1](https://latex.codecogs.com/gif.latex?%3D%20%5Csqrt%7B%28j_1%20%5Cmp%20m_1%20&plus;%201%29%28j_1%20%5Cpm%20m_1%29%7D%3Cj_1j_2%3B%20m_1%20%5Cmp%201%2C%20m_2%7Cj_1j_2%3B%20jm%3E)

![Equation 1 - right hand side part 2](https://latex.codecogs.com/gif.latex?&plus;%20%5Csqrt%7B%28j_2%20%5Cmp%20m_2%20&plus;%201%29%28j_2%20%5Cpm%20m_2%29%7D%3Cj_1j_2%3B%20m_1%2C%20m_2%20%5Cmp%201%7Cj_1j_2%3B%20jm%3E)

subject to the following constraints:

```
|j1 - j2| <= j <= j1 + j2  
m1 + m2 = m  
|m1| <= j1  
|m2| <= j2  
-j <= m1 + m2 <= j  
j1 and m1 must both simultaneously be integers or half integers  
j2 and m2 must both simultaneously be integers or half integers
```

Furthermore, the coefficients are subject to the overall
normalization equation:

![Equation 2 - normalization](https://latex.codecogs.com/gif.latex?%5Csum%20%5Csum%20%7C%3Cj_1j_2%3B%20m_1m_2%7Cj_1j_2%3B%20jm%3E%7C%5E2%20%3D%201)

The program follows the method outlined in [1](#ref-1)
to calculate the coefficients:

We start by setting the coefficient at the maximal
m1 + m2 value equal to 1. We then iterate through all valid
nodes of m1 and m2 values, using the values already set in
adjacent nodes, until the entire grid has been set. Finally,
we normalize the values and then obtain the requested
coefficient.

## Exact Radical Solution

One thing that nagged me about my original solution as
well as earlier versions of this solution was that they
produced answers as decimals. The trouble is that most tables
of Clebsch-Gordan coefficients (e.g., [here](https://en.wikipedia.org/wiki/Table_of_Clebsch%E2%80%93Gordan_coefficients))
provide them in radical form.

I wanted my program to calculate the coefficients and present
them as they're commonly seen in those tables. This meant that
I couldn't resort to decimal calculations - I needed to preserve
the radicals in each step and perform radical arithmetic. This
gave birth to my [Radicals](https://github.com/erjicles/Radicals)
NuGet package. With the Radicals package, I was finally able to
reproduce the coefficients as seen in most tables.

## References

<a name="ref-1"></a>[1]
Sakurai, J. J. 
"Recursion Relations for the Clebsch-Gordan Coefficients." 
*Modern Quantum Mechanics*. 
Ed. San F. Tuan. 
Reading: Addison Wesley Longman, 1994. 210-212. Print.
