module.exports = {
    root: true,
    env: { browser: true, es2020: true },
    extends: [
        'eslint:recommended',
        'plugin:@typescript-eslint/recommended',
        'plugin:react-hooks/recommended',
    ],
    ignorePatterns: ['dist', '.eslintrc.cjs'],
    parser: '@typescript-eslint/parser',
    plugins: ['react-refresh'],
    rules: {
        "no-unused-vars": "off",
        "eqeqeq": "off",
        'no-warning-comments': 'off',
        'react-refresh/only-export-components': [
            'warn',
            { allowConstantExport: true },
        ],
        '@typescript-eslint/no-explicit-any': 'off',
    },
    overrides: [
        {
            files: ['*.html'],
            parser: 'html-eslint-parser',
            plugins: ['html'],
            rules: {
                // i can add rules here
            }
        }
    ]
}
