using System;
using TMPro;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] TMP_InputField monthInput, dayInput, yearInput, hourInput, minutesInput, longValueInput, latValueInput;
    [SerializeField] TMP_Text altitudeText, azimuthText;
    [SerializeField] SunController sunController;

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        DateTime currentNow = DateTime.Now;

        monthInput.text = currentNow.Month.ToString();
        dayInput.text = currentNow.Day.ToString();
        yearInput.text = currentNow.Year.ToString();

        hourInput.text = currentNow.Hour.ToString();
        minutesInput.text = currentNow.Minute.ToString();

        DateTime utcNow = DateTime.UtcNow;
        Debug.Log(utcNow);
        Debug.Log(currentNow);

        UpdateUISun(0, 0);
    }

    #region HANDLE INPUT

    public void OnEndMonthInput(string value)
    {
        monthInput.text = GetNearestValidMonth(value);

        dayInput.text = GetNearestValidDay(dayInput.text);
    }

    private string GetNearestValidMonth(string value)
    {
        int month = (int)ConvertStringToFloat(value);
        if (month > 12) month = 12;
        else if (month < 1) month = 1;

        return month.ToString();
    }

    public void OnEndDayInput(string value)
    {
        dayInput.text = GetNearestValidDay(value);
    }

    private string GetNearestValidDay(string value)
    {
        int day = (int)ConvertStringToFloat(value);

        int currentYear = (int)ConvertStringToFloat(yearInput.text);
        int currentMonth = (int)ConvertStringToFloat(monthInput.text);

        int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
        if (day > daysInMonth)
        {
            day = daysInMonth;
        }

        if (currentMonth == 2 && day > 28)
        {
            if (DateTime.IsLeapYear(currentYear))
            {
                day = 29;
            }
            else
            {
                day = 28;
            }
        }

        return day.ToString();
    }

    public void OnEndYearInput(string value)
    {
        yearInput.text = GetNearestValidYear(value);

        dayInput.text = GetNearestValidDay(dayInput.text);
    }

    private string GetNearestValidYear(string value)
    {
        int year = (int)ConvertStringToFloat(value);
        if (year > 2100) year = 2100;
        else if (year < 1900) year = 1900;

        return year.ToString();
    }

    public void OnEndHourInput(string value)
    {
        int hour= (int)ConvertStringToFloat(value);
        if (hour > 23) hour = 23;
        else if (hour < 0) hour = 0;
        hourInput.text = hour.ToString();
    }

    public void OnEndMinutesInput(string value)
    {
        int minutes = (int)ConvertStringToFloat(value);

        if (minutes > 59) minutes = 59;
        else if (minutes < 0) minutes = 0;

        minutesInput.text = minutes.ToString();
    }

    public void OnEndUTCInput(string value)
    {
        float timeZone= ConvertStringToFloat(value);
        if (timeZone > 12) timeZone = 12;
        else if (timeZone < -12) timeZone = -12;
    }


    private float ConvertStringToFloat(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            Debug.LogError("Input string is empty.");
            return 0f; // Default value for empty input
        }

        if (float.TryParse(input, out float result))
        {
            return result;
        }
        else
        {
            Debug.LogError("Failed to convert string to float.");

            // Check if the input contains a minus sign
            if (input.Contains("-"))
            {
                // Remove the minus sign and try to convert again
                string cleanedInput = input.Replace("-", "");
                if (float.TryParse(cleanedInput, out float cleanedResult))
                {
                    return -cleanedResult; // Return the negative value
                }
            }

            return 0f; // Default value if conversion fails
        }
    }

    #endregion

    void UpdateUISun(double azi, double alt)
    {
        string degreeSymbol = "\u00B0"; // Unicode for the degree symbol
        altitudeText.text = string.Format("Sun Altitude: {0}{1}", (float)Math.Round(alt, 2) , degreeSymbol);
        azimuthText.text = string.Format("Sun Azimuth: {0}{1}", (float)Math.Round(azi, 2), degreeSymbol);
    }

    [SerializeField] PlanetCalculate planetCalculate;

    public void Calculate()
    {
        DateTime inputDateTime = new DateTime((int)ConvertStringToFloat(yearInput.text),
                                               (int)ConvertStringToFloat(monthInput.text),
                                               (int)ConvertStringToFloat(dayInput.text),
                                               (int)ConvertStringToFloat(hourInput.text),
                                               (int)ConvertStringToFloat(minutesInput.text), 0);
        Vector3 result = planetCalculate.CalculateSunPosition(inputDateTime, ConvertStringToFloat(latValueInput.text), ConvertStringToFloat(longValueInput.text));

        UpdateUISun(result.y,result.x);

        sunController.SetPosition(result.x, result.y);
    }
}
