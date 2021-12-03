import React from 'react';
import { Link } from 'react-router-dom';
import './badRequest.css';

const badRequest = () => {
	return (
		<div className='d-flex align-items-center flex-column bad-request'>
			<p className='bad-request'>
				Something's wrong... You might want to try one more time
				<span aria-label='frowning face with open mouth'>😦</span>
			</p>
			<button className='button default'>
				<Link className='bad-request' to={'/'}>
					Take me back to the homepage
				</Link>
			</button>
		</div>
	);
};

export default badRequest;
