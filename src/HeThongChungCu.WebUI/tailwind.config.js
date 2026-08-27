module.exports = {
  content: [
    './apps/**/*.{html,ts}',
    './libs/**/*.{html,ts}'
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        primary: '#136dec',
        'background-light': '#f6f7f8',
        'background-dark': '#101822'
      }
    },
  },
  variants: {
    extend: {},
  },
  plugins: [],
}
