import React from 'react';
import { unmountComponentAtNode } from 'react-dom';
import { act } from 'react-dom/test-utils';
import { render, fireEvent } from '@testing-library/react';
import App from './App';

let container = null;
const url = 'http://localhost/';

beforeEach(() => {
	container = document.createElement('div');
	document.body.appendChild(container);
});

afterEach(() => {
	unmountComponentAtNode(container);
	container.remove();
	container = null;
});

it('renders without crashing', () => {
	render(<App />, container);
});

it('navigates home when you click the logo', () => {
	render(<App />, container);

	act(() => {
		fireEvent.click(document.getElementsByClassName('navbar-brand')[0]);
	});

	expect(document.URL).toMatch(url);
});
