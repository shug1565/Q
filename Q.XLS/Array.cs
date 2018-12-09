using System;
using ManagedXLL;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Q.XLS
{
  // TODO: generic Array.Create

  [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
  public abstract class ArrayFunctions
  {
    [
        WorksheetFunction("Array.Expand", DisableInWizard = true),
        ExcelDescription("Returns the given array. Use to expand an array handle to actual cell values."),
    ]
    [return: ExcelArray(ReturnHandle = ExcelObjectHandle.Never)]
    public static Array Expand([ExcelDescription("The Array that is to be returned.")] Array input)
    {
      return input;
    }

    [
        WorksheetFunction("Array.GetValue"),
        ExcelDescription("Gets the value at the specified position in the multidimensional Array.")
    ]
    public static object GetValue(

        [ExcelDescription("The Array to get the value from.")]
            Array array,

        [ExcelDescription("The index specifying the position of the Array element to get.")]
            [ExcelAbbreviation("index")] params int[] indices)

    {
      return array.GetValue(indices);
    }

    [
        WorksheetFunction("Array.SetValue"),
        ExcelDescription("Sets the value at the specified position in the multidimensional Array.")
    ]
    public static void SetValue(

        [ExcelDescription("The Array to set the value in.")]
            Array array,

        [ExcelDescription("The value to be set.")]
            object value,

        [ExcelDescription("The index specifying the position of the Array element to get.")]
            [ExcelAbbreviation("index")] params int[] indices)

    {
      array.SetValue(value, indices);
    }


    [
        WorksheetFunction("Array.Length", DisableInWizard = true),
        ExcelDescription("Gets the total number of elements in all the dimensions of the Array.")
    ]
    public static int Length([ExcelDescription("The Array whose length needs to be determined.")] Array array)
    {
      return array.Length;
    }

    [
        WorksheetFunction("Array.Rank", DisableInWizard = true),
        ExcelDescription("Gets the rank (number of dimensions) of the Array.")
    ]
    public static int Rank([ExcelDescription("The Array whose rank needs to be determined.")] Array array)
    {
      return array.Rank;
    }

    /// <summary>
    /// Gets the number of elements in the specified dimension of the Array.
    /// </summary>
    [
        WorksheetFunction("Array.GetLength", DisableInWizard = true),
        ExcelDescription("Gets the number of elements in the specified dimension of the Array.")
    ]
    public static int GetLength(

        [ExcelDescription("The Array whose length needs to be determined.")]
            Array array,

        [ExcelDescription("A zero-based dimension of the Array whose length needs to be determined.")]
            int dimension)

    {
      return array.GetLength(dimension);
    }

    [
        WorksheetFunction("Array.BinarySearch", DisableInWizard = true),
        ExcelDescription("Search a sorted vector using a binary search algorithm. " +
            "Returns the index if value is found -or- a negative number, which is " +
            "the bitwise complement of the index of the first element that is larger than value.")
    ]
    public static int BinarySearch(
        [ExcelDescription("The one-dimensional array to search.")]
            Array array,
        [ExcelDescription("The value to search in array.")]
            [NoRef]XlOper value,
        [ExcelDescription("The starting index of the search. Defaults to 0.")]
            int startIndex = 0,
        [ExcelDescription("The number of elements in the section to search. Defaults to all elements beginning with startIndex.")]
            int count = int.MaxValue)
    {
      if (count == int.MaxValue)
        count = array.Length - startIndex;
      return Array.BinarySearch(array, startIndex, count, value.Value);
    }


    [
        WorksheetFunction("Array.Contains", DisableInWizard = true),
        ExcelDescription("Determines whether the Array contains a specific value.")
    ]
    public static bool Contains(
        [ExcelDescription("The one-dimensional array to search.")]
            Array array,
        [ExcelDescription("The value to locate in array.")]
            [NoRef]XlOper value)
    {
      return ((IList)array).Contains(value.Value);
    }


    [
        WorksheetFunction("Array.IndexOf", DisableInWizard = true),
        ExcelDescription("Returns the index of the first occurrence of a value in " +
            "a one-dimensional Array or in a portion of the Array. " +
            "Returns the index of the first occurrence of value if found; " +
            "otherwise, the lower bound of the array - 1.")
    ]
    public static int IndexOf(
        [ExcelDescription("The one-dimensional array to search.")]
            Array array,
        [ExcelDescription("The value to locate in array.")]
            [NoRef]XlOper value,
        [ExcelDescription("The starting index of the search. Defaults to 0.")]
            int startIndex = 0,
        [ExcelDescription("The number of elements in the section to search. Defaults to all alements beginning with startIndex.")]
            int count = int.MaxValue)
    {
      if (count == int.MaxValue)
        count = array.Length - startIndex;
      return Array.IndexOf(array, value.Value, startIndex, count);
    }

    [
        WorksheetFunction("Array.LastIndexOf", DisableInWizard = true),
        ExcelDescription("Returns the index of the last occurrence of a value in " +
            "a one-dimensional Array or in a portion of the Array." +
            "Returns the index of the first occurrence of value if found; " +
            "otherwise, the lower bound of the array - 1.")
    ]
    public static int LastIndexOf(
        [ExcelDescription("The one-dimensional array to search.")]
            Array array,
        [ExcelDescription("The value to locate in array.")]
            [NoRef]XlOper value,
        [ExcelDescription("The starting index of the backward search. Defaults to the last element.")]
            int startIndex = int.MaxValue,
        [ExcelDescription("The number of elements in the section to search. Defaults to all elements from 0 to startIndex.")]
            int count = int.MaxValue)
    {
      if (startIndex == int.MaxValue)
        startIndex = array.Length - 1;
      if (count == int.MaxValue)
        count = array.Length - startIndex;
      return Array.LastIndexOf(array, value.Value, startIndex, count);
    }

    [
        WorksheetFunction("Array.Clear", DisableInWizard = true),
        ExcelDescription("Sets all elements in the Array to zero, to false, or to a null reference " +
            "(Nothing in Visual Basic), depending on the element type.")
    ]
    public static void Clear(
        [ExcelDescription("The Array whose elements need to be cleared.")]
            Array array,
        [ExcelDescription("The starting index of the range of elements to clear. Defaults to 0.")]
            int index = 0,
        [ExcelDescription("The number of elements to clear. Defaults to all elements beginning with index.")]
            int length = int.MaxValue)
    {
      if (length == int.MaxValue)
        length = array.Length - index;
      Array.Clear(array, index, length);
    }


    [
        WorksheetFunction("Array.Clone", DisableInWizard = true),
        ExcelDescription("Creates a shallow copy of the Array.")
    ]
    public static Array Clone(
        [ExcelDescription("The Array to clone.")]
            Array array)
    {
      return (Array)array.Clone();
    }

    [
        WorksheetFunction("Array.Sort", DisableInWizard = true),
        ExcelDescription("Sorts a pair of one-dimensional Arrays based on the keys in the first Array")
    ]
    public static void Sort(
        [ExcelDescription("The one-dimensional Array that contains the keys to sort.")]
            Array keys,
        [ExcelDescription("An optional one-dimensional Array that contains the items that correspond to each of the keys.")]
            Array items = null,
        [ExcelDescription("The starting index of the range of elements to sort. Defaults to 0.")]
            int index = 0,
        [ExcelDescription("The number of elements to sort. Defaults to all the elements starting from index.")]
            int length = int.MaxValue)
    {
      if (length == int.MaxValue)
        length = keys.Length - index;
      Array.Sort(keys, items, index, length);
    }


    [
        WorksheetFunction("Array.Reverse", DisableInWizard = true),
        ExcelDescription("Reverses the sequence of the elements in a one-dimensional Array.")
    ]
    public static void Reverse(
        [ExcelDescription("The one-dimensional Array to reverse.")]
            Array array,
        [ExcelDescription("The starting index of the section to reverse. Defaults to 0.")]
            int index = 0,
        [ExcelDescription("The number of elements to reverse. Defaults to all elements beginning with index.")]
            int length = int.MaxValue)
    {
      if (length == int.MaxValue)
        length = array.Length - index;
      Array.Reverse(array, index, length);
    }


    [
        WorksheetFunction("Array.Copy", DisableInWizard = true),
        ExcelDescription("Copies a section of one Array to another Array " +
            "and performs type casting and boxing as required. " +
            "Returns the source array.")
    ]
    public static void Copy(
        [ExcelDescription("The Array that contains the data to copy.")]
            Array sourceArray,
        [ExcelDescription("The index in the sourceArray at which copying begins. Defaults to 0.")]
            int? sourceIndex,
        [ExcelDescription("The Array that receives the data.")]
            Array destinationArray,
        [ExcelDescription("The index in the destinationArray at which storing begins. Defaults to 0.")]
            int? destinationIndex,
        [ExcelDescription("The number of elements to copy. Defaults to the maximum elements.")]
            int? length)
    {
      int si = sourceIndex.GetValueOrDefault();
      int di = destinationIndex.GetValueOrDefault();
      int l = length ?? Math.Min(destinationArray.Length - di, sourceArray.Length - si);
      Array.Copy(sourceArray, si, destinationArray, di, l);
    }


  }
}
