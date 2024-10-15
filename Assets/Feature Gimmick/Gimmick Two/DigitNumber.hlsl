inline uint GetFigureNumbers(uint value)
{
	return floor(log10(value)) + 1;
}

inline uint GetDigitPlaceValue(uint value, uint figureNumbers)
{
	return (value / (uint)pow(10, figureNumbers - 1)) % 10;
}
