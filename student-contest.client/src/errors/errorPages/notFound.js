import React from 'react';
import { Link } from 'react-router-dom';
import './notFound.css';

const NotFound = () => {
	return (
		<div className='d-flex not-found'>
			<p className='not-found'>
				Ooops! The page you're looking for has been removed, renamed or
				unavailable
				<span aria-label='confused face'>😕</span>
			</p>
			<button className='button default'>
				<Link className='not-found' to={'/'}>
					Take me back to the homepage
				</Link>
			</button>
		</div>
	);
};

export default NotFound;
