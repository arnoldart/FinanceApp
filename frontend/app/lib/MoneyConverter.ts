export default function MoneyConverter(value: number | string) {
  const number = typeof value === "string" ? Number(value.replace(/\./g, '').replace(/,/g, '.')) : value

  if (Number.isNaN(number)) return '0'

  return new Intl.NumberFormat('id-ID').format(number)
}