import js from '@eslint/js';
import tseslint from 'typescript-eslint';
import react from 'eslint-plugin-react';
import reactHooks from 'eslint-plugin-react-hooks';
import jsxA11y from 'eslint-plugin-jsx-a11y';
import importPlugin from 'eslint-plugin-import';

/** @type {import('eslint').Linter.FlatConfig[]} */
export default [
    js.configs.recommended,
    ...tseslint.configs.recommended,
    {
        files: ['**/*.{ts,tsx,js,jsx}'],
        languageOptions: {
            ecmaVersion: 'latest',
            sourceType: 'module',
            parser: tseslint.parser,
            globals: {
                browser: true,
                node: true,
            },
        },
        plugins: {
            react,
            'react-hooks': reactHooks,
            'jsx-a11y': jsxA11y,
            import: importPlugin,
            'typescript-eslint': tseslint.plugin,
        },
        settings: {
            react: { version: 'detect' },
        },
        rules: {
            // ✅ Base JavaScript rules
            ...js.configs.recommended.rules,
            ...tseslint.configs.recommended[1].rules,
            ...react.configs.recommended.rules,
            ...reactHooks.configs.recommended.rules,
            ...jsxA11y.configs.recommended.rules,
            ...importPlugin.configs.recommended.rules,

            // ✅ Custom tweaks (relaxed rules)
            '@typescript-eslint/no-unused-vars': ['warn', { argsIgnorePattern: '^_' }],
            'import/order': 'off',

            // TypeScript strictness: relax
            '@typescript-eslint/no-explicit-any': 'off',
            '@typescript-eslint/ban-ts-comment': 'off',

            // React
            'react/react-in-jsx-scope': 'off',
            'react/prop-types': 'off',

            // React Hooks
            'react-hooks/exhaustive-deps': 'off', // Disable missing deps nagging

            // Accessibility (jsx-a11y)
            'jsx-a11y/label-has-associated-control': 'off',
            'jsx-a11y/anchor-is-valid': 'off',
            'jsx-a11y/click-events-have-key-events': 'off',
            'jsx-a11y/no-static-element-interactions': 'off',
            'jsx-a11y/aria-role': 'off',

            // Avoid errors from unresolved paths during lint
            'import/no-unresolved': 'off',
        },
    },
];
